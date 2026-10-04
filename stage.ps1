<#
.SYNOPSIS
Puts this repository back into the state the Stampeded! feature tours are written against.

.DESCRIPTION
Every staged branch is kept as a tag under stage/, so a branch that was merged, force-pushed
or deleted is one push away from what it was. Without a switch: main and the four branches go
back to their tags, a pull request is opened for every branch that has none open, and the
review comments are replaced by the seed ones.

Needs git, gh (logged in, with write access to the repository) and PowerShell 7.

.PARAMETER Push2
Force-pushes the rebased brand-registry branch over the one first pushed. Run it after
reading that pull request once: it is the push the re-review tour is about.

.PARAMETER Local
Creates local/dirty-work in this clone: a branch behind main with one commit and an
uncommitted edit on top. Nothing is pushed.
#>
param(
	[switch]$Push2,
	[switch]$Local
)

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

function Invoke-Tool {
	$exe, $rest = $args
	& $exe @rest
	if ($LASTEXITCODE -ne 0) {
		throw "$exe $($rest -join ' ') failed with exit code $LASTEXITCODE"
	}
}

# The line a seed comment goes on, found by its text in the staged revision: a line number
# written down here would be wrong the first time a staged commit is edited.
function Find-Line($rev, $path, $text) {
	$lines = Invoke-Tool git show "${rev}:$path"
	for ($i = 0; $i -lt $lines.Count; $i++) {
		if ($lines[$i].Contains($text)) { return $i + 1 }
	}
	throw "'$text' is not in $path at $rev"
}

function Add-Comment($repo, $number, $rev, $path, $text, $body) {
	$sha = Invoke-Tool git rev-parse "$rev^{commit}"
	$line = Find-Line $rev $path $text
	Invoke-Tool gh api "repos/$repo/pulls/$number/comments" `
		-f "body=$body" -f "commit_id=$sha" -f "path=$path" -F "line=$line" -f side=RIGHT --jq .id
}

# Every review comment on a pull request, newest first: a reply goes before what it answers.
function Remove-Comments($repo, $number) {
	$ids = @(Invoke-Tool gh api "repos/$repo/pulls/$number/comments" --paginate --jq '.[].id')
	[array]::Reverse($ids)
	foreach ($id in $ids) {
		Invoke-Tool gh api -X DELETE "repos/$repo/pulls/comments/$id" | Out-Null
	}
}

Invoke-Tool git fetch origin --tags --force --quiet

if ($Push2) {
	Invoke-Tool git push --force origin 'refs/tags/stage/brand-registry-v2:refs/heads/feature/brand-registry'
	Write-Host 'feature/brand-registry is now the rebased series (stage/brand-registry-v2).'
	return
}

if ($Local) {
	$branch = Invoke-Tool git branch --show-current
	if ((git status --porcelain) -and $branch -ne 'local/dirty-work') {
		throw "This clone has uncommitted changes on '$branch'. Commit or stash them first."
	}
	Invoke-Tool git switch --quiet --discard-changes -C local/dirty-work stage/local-work
	$herd = 'src/Corral/Herd.cs'
	$total = "`tpublic double TotalWeight() => animals.Sum(a => a.WeightKg);`n"
	$average = "`n`tpublic double AverageWeight() => animals.Count == 0 ? 0 : TotalWeight() / animals.Count;`n"
	$text = (Get-Content $herd -Raw).Replace("`r`n", "`n")
	if (-not $text.Contains($total)) { throw "$herd does not read as expected." }
	Set-Content $herd $text.Replace($total, $total + $average) -NoNewline
	Write-Host 'local/dirty-work is checked out: one commit behind main, one uncommitted edit.'
	return
}

$branches = [ordered]@{
	'feature/weight-pricing' = @{
		Tag = 'stage/weight-pricing'
		Title = 'Price herds by weight class'
		Body = @'
A single price per kilogram overpays for light animals and underpays for heavy ones. Buyers
sort cattle into three classes before they name a price, and this does the same:

| Class | Weight | Per kilogram |
| --- | --- | --- |
| Light | up to 350 kg | 90 % |
| Standard | 350 to 600 kg | 100 % |
| Heavy | above 600 kg | 110 % |

Worth reading commit by commit:

1. `Pricing` becomes `PriceCalculator` - a rename and nothing else.
2. The classes, and `PriceFor` following them.
3. The flat price goes. It was a guess at a weight, and with classes that is also a guess
   at a class. **This removes `--flat` from the command line.**
4. The report says how many animals of each brand fall into each class.

Closes #ISSUE
'@
	}
	'feature/brand-registry' = @{
		Tag = 'stage/brand-registry-v1'
		Title = 'Extract brand registry'
		Body = @'
A herd knew who owned every brand, so two herds of one ranch could disagree about it. Brand
ownership moves into `IBrandRegistry`, and a herd is handed one.

Also tightens what a brand is: it needs at least one letter now.
'@
	}
	'feature/herd-cache' = @{
		Tag = 'stage/herd-cache'
		Title = 'Cache herd totals'
		Body = @'
`HerdReport` asks for the total weight once per line, and each time the herd was summed
again. The total is kept until the herd changes.

Adds `Remove` and `Clear` while there, since both have to drop the cached total.
'@
	}
	'chore/cli-help' = @{
		Tag = 'stage/cli-help'
		Title = 'Fix typo in CLI help'
		Body = 'woth -> worth.'
	}
}

Invoke-Tool git push --force origin 'refs/tags/stage/main:refs/heads/main'
foreach ($branch in $branches.Keys) {
	Invoke-Tool git push --force origin "refs/tags/$($branches[$branch].Tag):refs/heads/$branch"
}

$repo = Invoke-Tool gh repo view --json nameWithOwner --jq .nameWithOwner
$numbers = @{}
foreach ($branch in $branches.Keys) {
	$number = Invoke-Tool gh pr list --head $branch --state open --json number --jq '.[0].number // empty'
	if (-not $number) {
		$stage = $branches[$branch]
		Invoke-Tool gh pr create --head $branch --base main --title $stage.Title --body $stage.Body | Out-Null
		$number = Invoke-Tool gh pr list --head $branch --state open --json number --jq '.[0].number // empty'
	}
	$numbers[$branch] = $number
	Write-Host "#$number $branch"
}

# The issue the first pull request closes. Opened after the pull requests, so that they keep
# the low numbers a reader types.
$issueTitle = 'Heavy animals sell for too little'
$issue = Invoke-Tool gh issue list --state open --json 'number,title' `
	--jq "map(select(.title == `"$issueTitle`")) | .[0].number // empty"
if (-not $issue) {
	# The search index lags behind a new issue; the URL gh prints does not.
	$issue = (Invoke-Tool gh issue create --title $issueTitle --body @'
`corral` prices every animal at the same rate per kilogram. The sale barn does not: a heavy
animal fetches about a tenth more per kilogram, a light one about a tenth less.
'@ | Select-Object -Last 1).Split('/')[-1]
}
$pricing = $numbers['feature/weight-pricing']
Invoke-Tool gh pr edit $pricing --body $branches['feature/weight-pricing'].Body.Replace('#ISSUE', "#$issue") | Out-Null

# The comments go back to the seed ones: whatever a tour, or a visitor, has said since is
# removed first, so that a second run leaves what the first one did.
foreach ($number in $numbers.Values) {
	Remove-Comments $repo $number
}
& {
	Add-Comment $repo $pricing 'stage/weight-pricing' 'src/Corral/PriceCalculator.cs' '< 350 => WeightClass.Light' @'
The table in `docs/pricing.md` says light is *up to* 350 kg, which reads as inclusive. Here a 350 kg animal is Standard. Which one is meant?
'@ | Out-Null
	$settled = Add-Comment $repo $pricing 'stage/weight-pricing' 'src/Corral/HerdReport.cs' '.OrderBy(g => g.Key)' @'
Is this the enum's order or alphabetical? Heavy before light would read oddly.
'@
	Invoke-Tool gh api "repos/$repo/pulls/$pricing/comments/$settled/replies" `
		-f "body=The enum's: Light, Standard, Heavy. The report test pins it." | Out-Null
	$owner, $name = $repo.Split('/')
	$thread = Invoke-Tool gh api graphql -f owner=$owner -f name=$name -F number=$pricing -f query='
		query($owner: String!, $name: String!, $number: Int!) {
			repository(owner: $owner, name: $name) { pullRequest(number: $number) {
				reviewThreads(first: 50) { nodes { id comments(first: 1) { nodes { databaseId } } } } } } }' `
		--jq ".data.repository.pullRequest.reviewThreads.nodes[] | select(.comments.nodes[0].databaseId == $settled) | .id"
	Invoke-Tool gh api graphql -f thread=$thread -f query='
		mutation($thread: ID!) { resolveReviewThread(input: { threadId: $thread }) { thread { isResolved } } }' | Out-Null
}

$registry = $numbers['feature/brand-registry']
& {
	Add-Comment $repo $registry 'stage/brand-registry-v1' 'src/Corral/Herd.cs' 'if (brand.All(char.IsAsciiDigit))' @'
The commit message says why an all-digit brand is refused; the code does not. The next reader will take this for a mistake and delete it.
'@ | Out-Null
}

Write-Host 'Staged.'
