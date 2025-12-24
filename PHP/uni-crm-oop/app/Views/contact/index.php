<?php
declare(strict_types=1);

use App\Core\Util;

$companyOptions = [['value'=>'','label'=>'All companies']];
foreach ($companies as $co) {
  $companyOptions[] = ['value'=>(string)$co['id'], 'label'=>(string)$co['name']];
}

$headers = ['Name','Company','Position','Contact','Created',''];
$rows = [];
foreach ($contacts as $c) {
  $rows[] = [
    Util::e(($c['first_name'] ?? '').' '.($c['last_name'] ?? '')),
    Util::e($c['company_name'] ?? ''),
    Util::e($c['position'] ?? ''),
    '<div>'.Util::e($c['email'] ?? '').'</div><div class="small">'.Util::e($c['phone'] ?? '').'</div>',
    '<span class="small">'.Util::e($c['created_at'] ?? '').'</span>',
    '<div class="actions">
        <a class="btn light" href="?r=contact/edit&id='.(int)$c['id'].'">Edit</a>
        <form method="post" action="?r=contact/delete" onsubmit="return confirm(\'Delete this contact?\');">
          <input type="hidden" name="csrf" value="'.Util::e($csrfToken).'" />
          <input type="hidden" name="id" value="'.(int)$c['id'].'" />
          <button class="btn danger" type="submit">Delete</button>
        </form>
     </div>'
  ];
}
?>
<div class="card">
  <div class="actions" style="justify-content:space-between;">
    <h1>Contacts</h1>
    <a class="btn" href="?r=contact/create">+ New</a>
  </div>

  <form method="get" class="actions" style="margin:10px 0;">
    <input type="hidden" name="r" value="contact/index" />
    <select name="company_id">
      <?php foreach ($companyOptions as $opt): ?>
        <option value="<?= Util::e($opt['value']) ?>" <?= ((string)($companyId ?? '') === (string)$opt['value']) ? 'selected' : '' ?>>
          <?= Util::e($opt['label']) ?>
        </option>
      <?php endforeach; ?>
    </select>
    <button class="btn" type="submit">Filter</button>
    <?php if (!empty($companyId)): ?><a class="btn light" href="?r=contact/index">Clear</a><?php endif; ?>
  </form>

  <?php
    $emptyText = 'No contacts yet.';
    require __DIR__ . '/../partials/table.php';
  ?>
</div>
