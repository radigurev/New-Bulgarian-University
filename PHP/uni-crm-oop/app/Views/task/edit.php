<?php
declare(strict_types=1);

use App\Core\Util;

$companyOptions = [['value'=>'','label'=>'(none)']];
foreach ($companies as $co) $companyOptions[] = ['value'=>(string)$co['id'], 'label'=>(string)$co['name']];

$dealOptions = [['value'=>'','label'=>'(none)']];
foreach ($deals as $d) {
  $dealOptions[] = ['value'=>(string)$d['id'], 'label'=>($d['company_name'] ?? '').' - '.($d['title'] ?? '')];
}

$statusOptions = [
  ['value'=>'Open','label'=>'Open'],
  ['value'=>'Done','label'=>'Done'],
];

$fields = [
  ['name'=>'title','label'=>'Title','type'=>'text','required'=>true,'value'=>($_POST['title'] ?? $task['title'] ?? '')],
  ['name'=>'status','label'=>'Status','type'=>'select','value'=>($_POST['status'] ?? $task['status'] ?? 'Open'),'options'=>$statusOptions],
  ['name'=>'due_date','label'=>'Due date','type'=>'date','value'=>($_POST['due_date'] ?? $task['due_date'] ?? '')],
  ['name'=>'company_id','label'=>'Company','type'=>'select','value'=>($_POST['company_id'] ?? $task['company_id'] ?? ''),'options'=>$companyOptions],
  ['name'=>'deal_id','label'=>'Deal','type'=>'select','value'=>($_POST['deal_id'] ?? $task['deal_id'] ?? ''),'options'=>$dealOptions],
];
?>
<div class="card">
  <h1>Edit task</h1>
  <div class="small"><?= Util::e($task['title'] ?? '') ?></div>
  <hr />
  <?php if (!empty($error)): ?><div class="flash err"><?= Util::e($error) ?></div><?php endif; ?>

  <?php
    $action = '?r=task/edit&id='.(int)$task['id'];
    $submitLabel = 'Save';
    $cancelUrl = '?r=task/index';
    require __DIR__ . '/../partials/form.php';
  ?>
</div>
