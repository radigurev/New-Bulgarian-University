<?php
declare(strict_types=1);

use App\Core\Util;

$stages = [
  ['value'=>'New','label'=>'New'],
  ['value'=>'Qualified','label'=>'Qualified'],
  ['value'=>'Proposal','label'=>'Proposal'],
  ['value'=>'Won','label'=>'Won'],
  ['value'=>'Lost','label'=>'Lost'],
];

$fields = [
  ['name'=>'title','label'=>'Title','type'=>'text','required'=>true,'value'=>($_POST['title'] ?? $deal['title'] ?? '')],
  ['name'=>'amount','label'=>'Amount','type'=>'number','value'=>($_POST['amount'] ?? $deal['amount'] ?? ''),'placeholder'=>'e.g. 1200.00'],
  ['name'=>'stage','label'=>'Stage','type'=>'select','value'=>($_POST['stage'] ?? $deal['stage'] ?? 'New'),'options'=>$stages],
  ['name'=>'expected_close_date','label'=>'Expected close date','type'=>'date','value'=>($_POST['expected_close_date'] ?? $deal['expected_close_date'] ?? '')],
];
?>
<div class="card">
  <h1>Edit deal</h1>
  <div class="small"><?= Util::e($deal['company_name'] ?? '') ?></div>
  <hr />
  <?php if (!empty($error)): ?><div class="flash err"><?= Util::e($error) ?></div><?php endif; ?>

  <?php
    $action = '?r=deal/edit&id='.(int)$deal['id'];
    $submitLabel = 'Save';
    $cancelUrl = '?r=deal/index';
    $columns = 2;
    require __DIR__ . '/../partials/form.php';
  ?>
</div>
