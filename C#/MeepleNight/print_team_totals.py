import json, os, sys, io
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
ROOT = os.path.dirname(os.path.abspath(__file__))
RAW = open(os.path.join(ROOT, 'match_details.json'), encoding='utf-8').read()
matches_by_div = json.loads(json.loads(RAW))
for div in ['A','B','C']:
    teams = {}
    team_matches = {}
    for m in matches_by_div[div]:
        for team in m['teams']:
            tn = team['teamName']
            team_matches.setdefault(tn, set()).add(m['id'])
            if tn not in teams:
                teams[tn] = dict(ace=0, attack=0, block=0, serviceError=0, foul=0, error=0)
            for p in team['players']:
                for k in ('ace','attack','block','serviceError','foul','error'):
                    teams[tn][k] += p[k]
    print(f'\n=== ДИВИЗИЯ {div} ===')
    rows = []
    for tn, t in teams.items():
        pts = t['ace']+t['attack']+t['block']
        errs = t['serviceError']+t['foul']+t['error']
        rows.append((tn, len(team_matches[tn]), pts, t['attack'], t['block'], t['ace'], errs, t['serviceError'], t['foul'], t['error']))
    rows.sort(key=lambda x: -x[2])
    print(f"{'Отбор':<24} {'Мач':>3} {'Точки':>6} {'Атки':>5} {'Блок':>5} {'Аса':>4} {'Грешки':>7} {'Серв':>5} {'Нар':>4} {'Др':>4}")
    for r in rows:
        print(f"{r[0]:<24} {r[1]:>3} {r[2]:>6} {r[3]:>5} {r[4]:>5} {r[5]:>4} {r[6]:>7} {r[7]:>5} {r[8]:>4} {r[9]:>4}")
