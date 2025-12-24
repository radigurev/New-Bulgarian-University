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

$selectedCompany = (string)($_POST['company_id'] ?? ($prefCompanyId ?? ''));

$fields = [
  ['name'=>'title','label'=>'Title','type'=>'text','required'=>true,'value'=>($_POST['title'] ?? '')],
  ['name'=>'status','label'=>'Status','type'=>'select','value'=>($_POST['status'] ?? 'Open'),'options'=>$statusOptions],
  ['name'=>'due_date','label'=>'Due date','type'=>'date','value'=>($_POST['due_date'] ?? '')],
  ['name'=>'company_id','label'=>'Company','type'=>'select','value'=>$selectedCompany,'options'=>$companyOptions],
  ['name'=>'deal_id','label'=>'Deal','type'=>'select','value'=>($_POST['deal_id'] ?? ''),'options'=>$dealOptions],
];
?>
<div class="card">
  <h1>New task</h1><hr />
  <?php if (!empty($error)): ?><div class="flash err"><?= Util::e($error) ?></div><?php endif; ?>

  <?php
    $action = '?r=task/create';
    $submitLabel = 'Create';
    $cancelUrl = '?r=task/index';
    require __DIR__ . '/../partials/form.php';
  ?>
</div>
