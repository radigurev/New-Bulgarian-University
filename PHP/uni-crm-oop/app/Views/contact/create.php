<?php
declare(strict_types=1);

use App\Core\Util;

$options = [];
foreach ($companies as $co) {
  $options[] = ['value'=>(string)$co['id'], 'label'=>(string)$co['name']];
}

$selectedCompany = (string)($_POST['company_id'] ?? ($prefCompanyId ?? ''));

$fields = [
  ['name'=>'company_id','label'=>'Company','type'=>'select','required'=>true,'value'=>$selectedCompany,'options'=>$options],
  ['name'=>'first_name','label'=>'First name','type'=>'text','required'=>true,'value'=>($_POST['first_name'] ?? '')],
  ['name'=>'last_name','label'=>'Last name','type'=>'text','required'=>true,'value'=>($_POST['last_name'] ?? '')],
  ['name'=>'position','label'=>'Position','type'=>'text','value'=>($_POST['position'] ?? '')],
  ['name'=>'email','label'=>'Email','type'=>'email','value'=>($_POST['email'] ?? '')],
  ['name'=>'phone','label'=>'Phone','type'=>'text','value'=>($_POST['phone'] ?? '')],
];
?>
<div class="card">
  <h1>New contact</h1><hr />
  <?php if (!empty($error)): ?><div class="flash err"><?= Util::e($error) ?></div><?php endif; ?>

  <?php
    $action = '?r=contact/create';
    $submitLabel = 'Create';
    $cancelUrl = !empty($prefCompanyId) ? ('?r=company/view&id='.(int)$prefCompanyId) : '?r=contact/index';
    require __DIR__ . '/../partials/form.php';
  ?>
</div>
