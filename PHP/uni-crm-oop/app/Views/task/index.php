<?php
declare(strict_types=1);

use App\Core\Util;

$statuses = ['' => 'All', 'Open'=>'Open', 'Done'=>'Done'];

$headers = ['Title','Company','Deal','Status','Due','Created',''];
$rows = [];
foreach ($tasks as $t) {
  $status = (string)($t['status'] ?? '');
  $cls = $status === 'Done' ? 'green' : 'red';
  $rows[] = [
    Util::e($t['title'] ?? ''),
    Util::e($t['company_name'] ?? ''),
    Util::e($t['deal_title'] ?? ''),
    '<span class="badge '.$cls.'">'.Util::e($status).'</span>',
    Util::e($t['due_date'] ?? ''),
    '<span class="small">'.Util::e($t['created_at'] ?? '').'</span>',
    '<div class="actions">
        <a class="btn light" href="?r=task/edit&id='.(int)$t['id'].'">Edit</a>
        <form method="post" action="?r=task/delete" onsubmit="return confirm(\'Delete this task?\');">
          <input type="hidden" name="csrf" value="'.Util::e($csrfToken).'" />
          <input type="hidden" name="id" value="'.(int)$t['id'].'" />
          <button class="btn danger" type="submit">Delete</button>
        </form>
     </div>'
  ];
}
?>
<div class="card">
  <div class="actions" style="justify-content:space-between;">
    <h1>Tasks</h1>
    <a class="btn" href="?r=task/create">+ New</a>
  </div>

  <form method="get" class="actions" style="margin:10px 0;">
    <input type="hidden" name="r" value="task/index" />
    <select name="status">
      <?php foreach ($statuses as $key => $label): ?>
        <option value="<?= Util::e($key) ?>" <?= ((string)($status ?? '') === (string)$key) ? 'selected' : '' ?>>
          <?= Util::e($label) ?>
        </option>
      <?php endforeach; ?>
    </select>
    <button class="btn" type="submit">Filter</button>
    <?php if (!empty($status)): ?><a class="btn light" href="?r=task/index">Clear</a><?php endif; ?>
  </form>

  <?php
    $emptyText = 'No tasks yet.';
    require __DIR__ . '/../partials/table.php';
  ?>
</div>
