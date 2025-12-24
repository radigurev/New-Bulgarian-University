<?php
declare(strict_types=1);
?>
<div class="card">
  <h1>Dashboard</h1>
  <hr />
  <div class="actions">
    <span class="badge blue">Companies: <?= (int)$companyCount ?></span>
    <span class="badge blue">Deals: <?= (int)$dealCount ?></span>
    <span class="badge <?= ((int)$openTasks > 0) ? 'red' : 'green' ?>">Open tasks: <?= (int)$openTasks ?></span>
  </div>
</div>

<div class="card">
  <h2>Quick actions</h2>
  <div class="actions">
    <a class="btn" href="?r=company/create">+ Company</a>
    <a class="btn" href="?r=contact/create">+ Contact</a>
    <a class="btn" href="?r=deal/create">+ Deal</a>
    <a class="btn" href="?r=task/create">+ Task</a>
  </div>
</div>
