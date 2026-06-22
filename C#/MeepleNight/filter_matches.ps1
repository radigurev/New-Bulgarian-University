[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$raw = [System.IO.File]::ReadAllText("$PSScriptRoot\matches.json", [System.Text.Encoding]::UTF8)
$arr = $raw | ConvertFrom-Json

# Season-6 male team names per division (as displayed in standings)
$divA = @('ВЛАДО ВОЛЕЙ (М)','SPIKERS 1','THE INCREDIBALLS','HEATWAVE','СЕРДИКА ВОЛЕЙ 1')
$divB = @('SPIKERS 2','MONGOOSE  SPIKE','MONGOOSE SPIKE','СЕРДИКА ВОЛЕЙ 2','ШИП','САМОКОВ ВОЛЕЙ','САМУРАЙСКИТЕ СУРИКАТИ','WE VOLLEY','ККС')
$divC = @('СВОГЕ (М)','АКАДЕМИК U16 (М)','ЛИМОНИТЕ НА МЛАДОСТ','ЛАЗАРЕТ','ФЕНИКС','МАНТИНЕЛА')

# Filter to season 6 window
$cutoff = [datetime]'2026-01-15T00:00:00Z'
$s6 = $arr | Where-Object { [datetime]$_.startTime -gt $cutoff }
Write-Host "Matches after 2026-01-15: $($s6.Count)"

function GetDiv($t1, $t2) {
  $n = @($t1, $t2)
  if (($n | Where-Object { $divA -contains $_ }).Count -eq 2) { return 'A' }
  if (($n | Where-Object { $divB -contains $_ }).Count -eq 2) { return 'B' }
  if (($n | Where-Object { $divC -contains $_ }).Count -eq 2) { return 'C' }
  return $null
}

$buckets = @{ A=@(); B=@(); C=@() }
$unmatched = @()
foreach ($m in $s6) {
  $n1 = $m.teams[0].name
  $n2 = $m.teams[1].name
  $d = GetDiv $n1 $n2
  if ($d) {
    $buckets[$d] += $m
  } else {
    # Check if either team appears in a male division — if so, the other team is interesting
    $allMale = $divA + $divB + $divC
    if ($allMale -contains $n1 -or $allMale -contains $n2) {
      $unmatched += [pscustomobject]@{ id=$m.id; title=$m.title; t1=$n1; id1=$m.teams[0].id; t2=$n2; id2=$m.teams[1].id; date=$m.startTime }
    }
  }
}

"DIV A count: $($buckets['A'].Count) (expect 20)"
"DIV B count: $($buckets['B'].Count) (expect 28)"
"DIV C count: $($buckets['C'].Count) (expect 30)"
"Unmatched (one side is a known male season-6 team):"
$unmatched | Format-Table -AutoSize

# Save splits
foreach ($k in 'A','B','C') {
  $ids = $buckets[$k] | ForEach-Object { $_.id }
  $ids | ConvertTo-Json -Compress | Set-Content "$PSScriptRoot\match_ids_$k.json" -Encoding UTF8
}

# Save team-id-to-name map from buckets
$teamMap = @{}
foreach ($k in 'A','B','C') {
  foreach ($m in $buckets[$k]) {
    foreach ($t in $m.teams) { $teamMap[[string]$t.id] = $t.name }
  }
}
$teamMap | ConvertTo-Json | Set-Content "$PSScriptRoot\team_map.json" -Encoding UTF8
"Team-id-to-name map (season 6, male divisions):"
$teamMap | Format-Table -AutoSize
