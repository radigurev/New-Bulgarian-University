<?php
declare(strict_types=1);

use App\Core\Util;

$stages = ['' => 'All stages', 'New'=>'New', 'Qualified'=>'Qualified', 'Proposal'=>'Proposal', 'Won'=>'Won', 'Lost'=>'Lost'];

$headers = ['Title','Company','Stage','Amount','Expected','Created',''];
$rows = [];
foreach ($deals as $d) {
  $rows[] = [
    Util::e($d['title'] ?? ''),
    Util::e($d['company_name'] ?? ''),
    '<span class="badge">'.Util::e($d['stage'] ?? '').'</span>',
    Util::e($d['amount'] ?? ''),
    Util::e($d['expected_close_date'] ?? ''),
    '<span class="small">'.Util::e($d['created_at'] ?? '').'</span>',
    '<div class="actions">
        <a class="btn light" href="?r=deal/edit&id='.(int)$d['id'].'">Edit</a>
        <form method="post" action="?r=deal/delete" onsubmit="return confirm(\'Delete this deal?\');">
          <input type="hidden" name="csrf" value="'.Util::e($csrfToken).'" />
          <input type="hidden" name="id" value="'.(int)$d['id'].'" />
          <button class="btn danger" type="submit">Delete</button>
        </form>
     </div>'
  ];
}
?>
<div class="card">
  <div class="actions" style="justify-content:space-between;">
    <h1>Deals</h1>
    <a class="btn" href="?r=deal/create">+ New</a>
  </div>

  <form method="get" class="actions" style="margin:10px 0;">
    <input type="hidden" name="r" value="deal/index" />
    <select name="stage">
      <?php foreach ($stages as $key => $label): ?>
        <option value="<?= Util::e($key) ?>" <?= ((string)($stage ?? '') === (string)$key) ? 'selected' : '' ?>>
          <?= Util::e($label) ?>
        </option>
      <?php endforeach; ?>
    </select>
    <button class="btn" type="submit">Filter</button>
    <?php if (!empty($stage)): ?><a class="btn light" href="?r=deal/index">Clear</a><?php endif; ?>
  </form>

  <?php
    $emptyText = 'No deals yet.';
    require __DIR__ . '/../partials/table.php';
  ?>
</div>
