<?php
declare(strict_types=1);

use App\Core\Util;

$headers = ['Name','Contact','VAT','Created',''];
$rows = [];

foreach ($companies as $c) {
  $rows[] = [
    '<a href="?r=company/view&id='.(int)$c['id'].'">'.Util::e($c['name']).'</a>',
    '<div>'.Util::e($c['email'] ?? '').'</div><div class="small">'.Util::e($c['phone'] ?? '').'</div>',
    Util::e($c['vat_number'] ?? ''),
    '<span class="small">'.Util::e($c['created_at'] ?? '').'</span>',
    '<div class="actions">
        <a class="btn light" href="?r=company/edit&id='.(int)$c['id'].'">Edit</a>
        <form method="post" action="?r=company/delete" onsubmit="return confirm(\'Delete this company?\');">
          <input type="hidden" name="csrf" value="'.Util::e($csrfToken).'" />
          <input type="hidden" name="id" value="'.(int)$c['id'].'" />
          <button class="btn danger" type="submit">Delete</button>
        </form>
     </div>'
  ];
}
?>
<div class="card">
  <h1>Companies</h1>

  <form method="get" class="actions" style="margin:10px 0;">
    <input type="hidden" name="r" value="company/index" />
    <input name="q" placeholder="Search..." value="<?= Util::e($q ?? '') ?>" />
    <button class="btn" type="submit">Search</button>
    <a class="btn secondary" href="?r=company/create">+ New</a>
  </form>

  <?php
    $emptyText = 'No companies yet.';
    require __DIR__ . '/../partials/table.php';
  ?>
</div>
