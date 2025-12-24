<?php
declare(strict_types=1);

use App\Core\Util;

$companyOptions = [];
foreach ($companies as $co) $companyOptions[] = ['value'=>(string)$co['id'], 'label'=>(string)$co['name']];

$contactOptions = [['value'=>'','label'=>'(none)']];
foreach ($contacts as $c) {
  $label = ($c['company_name'] ?? '') . ' - ' . ($c['name'] ?? '');
  $contactOptions[] = ['value'=>(string)$c['id'], 'label'=>$label];
}

$selectedCompany = (string)($_POST['company_id'] ?? ($prefCompanyId ?? ''));
$stages = [
  ['value'=>'New','label'=>'New'],
  ['value'=>'Qualified','label'=>'Qualified'],
  ['value'=>'Proposal','label'=>'Proposal'],
  ['value'=>'Won','label'=>'Won'],
  ['value'=>'Lost','label'=>'Lost'],
];

$fields = [
  ['name'=>'company_id','label'=>'Company','type'=>'select','required'=>true,'value'=>$selectedCompany,'options'=>$companyOptions],
  ['name'=>'contact_id','label'=>'Contact','type'=>'select','value'=>($_POST['contact_id'] ?? ''),'options'=>$contactOptions],
  ['name'=>'title','label'=>'Title','type'=>'text','required'=>true,'value'=>($_POST['title'] ?? '')],
  ['name'=>'amount','label'=>'Amount','type'=>'number','value'=>($_POST['amount'] ?? ''),'placeholder'=>'e.g. 1200.00'],
  ['name'=>'stage','label'=>'Stage','type'=>'select','value'=>($_POST['stage'] ?? 'New'),'options'=>$stages],
  ['name'=>'expected_close_date','label'=>'Expected close date','type'=>'date','value'=>($_POST['expected_close_date'] ?? '')],
];
?>
<div class="card">
  <h1>New deal</h1><hr />
  <?php if (!empty($error)): ?><div class="flash err"><?= Util::e($error) ?></div><?php endif; ?>

  <?php
    $action = '?r=deal/create';
    $submitLabel = 'Create';
    $cancelUrl = '?r=deal/index';
    require __DIR__ . '/../partials/form.php';
  ?>
</div>
