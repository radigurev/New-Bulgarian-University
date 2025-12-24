<?php
declare(strict_types=1);

use App\Core\Util;
?>
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width,initial-scale=1" />
  <title>Uni CRM</title>
  <link rel="stylesheet" href="assets/style.css" />
</head>
<body>
  <?php if (!empty($currentUser)): ?>
  <div class="topbar">
    <div class="nav">
      <a class="brand" href="?r=dashboard/index">Uni CRM</a>
      <a href="?r=company/index">Companies</a>
      <a href="?r=contact/index">Contacts</a>
      <a href="?r=deal/index">Deals</a>
      <a href="?r=task/index">Tasks</a>
      <span class="spacer"></span>
      <span class="small">Hi, <?= Util::e($currentUser['full_name'] ?? '') ?></span>
      <a class="btn light" href="?r=auth/logout">Logout</a>
    </div>
  </div>
  <?php endif; ?>

  <div class="container">
    <?php if (!empty($flash)): ?>
      <div class="flash <?= Util::e($flash['type']) ?>"><?= Util::e($flash['message']) ?></div>
    <?php endif; ?>

    <?php require $contentFile; ?>
  </div>
</body>
</html>
