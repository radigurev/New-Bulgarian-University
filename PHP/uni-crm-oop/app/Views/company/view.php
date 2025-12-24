<?php
declare(strict_types=1);

use App\Core\Util;

// Contacts table
$contactHeaders = ['Name','Position','Contact',''];
$contactRows = [];
foreach ($contacts as $c) {
  $contactRows[] = [
    Util::e(($c['first_name'] ?? '').' '.($c['last_name'] ?? '')),
    Util::e($c['position'] ?? ''),
    '<div>'.Util::e($c['email'] ?? '').'</div><div class="small">'.Util::e($c['phone'] ?? '').'</div>',
    '<a class="btn light" href="?r=contact/edit&id='.(int)$c['id'].'">Edit</a>',
  ];
}

// Deals table
$dealHeaders = ['Title','Stage','Amount','Expected',''];
$dealRows = [];
foreach ($deals as $d) {
  $dealRows[] = [
    Util::e($d['title'] ?? ''),
    '<span class="badge">'.Util::e($d['stage'] ?? '').'</span>',
    Util::e($d['amount'] ?? ''),
    Util::e($d['expected_close_date'] ?? ''),
    '<a class="btn light" href="?r=deal/edit&id='.(int)$d['id'].'">Edit</a>',
  ];
}

// Tasks table
$taskHeaders = ['Title','Status','Due',''];
$taskRows = [];
foreach ($tasks as $t) {
  $status = (string)($t['status'] ?? '');
  $cls = $status === 'Done' ? 'green' : 'red';
  $taskRows[] = [
    Util::e($t['title'] ?? ''),
    '<span class="badge '.$cls.'">'.Util::e($status).'</span>',
    Util::e($t['due_date'] ?? ''),
    '<a class="btn light" href="?r=task/edit&id='.(int)$t['id'].'">Edit</a>',
  ];
}
?>
<div class="card">
  <div class="actions" style="justify-content:space-between;">
    <div>
      <h1><?= Util::e($company['name'] ?? '') ?></h1>
      <div class="small">
        <?= Util::e($company['email'] ?? '') ?>
        <?= ($company['phone'] ?? '') ? ' • ' . Util::e($company['phone']) : '' ?>
        <?= ($company['vat_number'] ?? '') ? ' • VAT: ' . Util::e($company['vat_number']) : '' ?>
      </div>
    </div>
    <div class="actions">
      <a class="btn light" href="?r=company/edit&id=<?= (int)$company['id'] ?>">Edit</a>
      <a class="btn secondary" href="?r=company/index">Back</a>
    </div>
  </div>

  <?php if (($company['website'] ?? '') || ($company['address'] ?? '')): ?>
    <hr />
    <div class="small">
      <?php if (($company['website'] ?? '')): ?>Website: <?= Util::e($company['website']) ?><br><?php endif; ?>
      <?php if (($company['address'] ?? '')): ?>Address: <?= Util::e($company['address']) ?><?php endif; ?>
    </div>
  <?php endif; ?>
</div>

<div class="card">
  <div class="actions" style="justify-content:space-between;">
    <h2>Contacts</h2>
    <a class="btn" href="?r=contact/create&company_id=<?= (int)$company['id'] ?>">+ Contact</a>
  </div>
  <?php
    $headers = $contactHeaders; $rows = $contactRows; $emptyText = 'No contacts yet.';
    require __DIR__ . '/../partials/table.php';
  ?>
</div>

<div class="card">
  <div class="actions" style="justify-content:space-between;">
    <h2>Deals</h2>
    <a class="btn" href="?r=deal/create&company_id=<?= (int)$company['id'] ?>">+ Deal</a>
  </div>
  <?php
    $headers = $dealHeaders; $rows = $dealRows; $emptyText = 'No deals yet.';
    require __DIR__ . '/../partials/table.php';
  ?>
</div>

<div class="card">
  <div class="actions" style="justify-content:space-between;">
    <h2>Tasks</h2>
    <a class="btn" href="?r=task/create&company_id=<?= (int)$company['id'] ?>">+ Task</a>
  </div>
  <?php
    $headers = $taskHeaders; $rows = $taskRows; $emptyText = 'No tasks.';
    require __DIR__ . '/../partials/table.php';
  ?>
</div>

<div class="card">
  <h2>Notes</h2>
  <form method="post" action="?r=note/createForCompany">
    <input type="hidden" name="csrf" value="<?= Util::e($csrfToken) ?>" />
    <input type="hidden" name="company_id" value="<?= (int)$company['id'] ?>" />
    <label>Add note</label>
    <textarea name="body" placeholder="Write a note..."></textarea>
    <div class="actions" style="margin-top:10px;">
      <button class="btn" type="submit">Add</button>
    </div>
  </form>

  <hr />
  <?php foreach ($notes as $n): ?>
    <div class="card" style="box-shadow:none;border:1px solid #e5e7eb;">
      <div class="small"><?= Util::e($n['created_at'] ?? '') ?> • <?= Util::e($n['full_name'] ?? '') ?></div>
      <div style="white-space:pre-wrap;margin-top:6px;"><?= Util::e($n['body'] ?? '') ?></div>
    </div>
  <?php endforeach; ?>
  <?php if (!$notes): ?><div class="small">No notes yet.</div><?php endif; ?>
</div>
