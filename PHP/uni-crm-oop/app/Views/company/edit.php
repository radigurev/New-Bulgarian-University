<?php
declare(strict_types=1);

use App\Core\Util;

$fields = [
  ['name'=>'name','label'=>'Name','type'=>'text','required'=>true,'value'=>($_POST['name'] ?? $company['name'] ?? '')],
  ['name'=>'vat_number','label'=>'VAT','type'=>'text','value'=>($_POST['vat_number'] ?? $company['vat_number'] ?? '')],
  ['name'=>'email','label'=>'Email','type'=>'email','value'=>($_POST['email'] ?? $company['email'] ?? '')],
  ['name'=>'phone','label'=>'Phone','type'=>'text','value'=>($_POST['phone'] ?? $company['phone'] ?? '')],
  ['name'=>'website','label'=>'Website','type'=>'text','value'=>($_POST['website'] ?? $company['website'] ?? '')],
  ['name'=>'address','label'=>'Address','type'=>'text','value'=>($_POST['address'] ?? $company['address'] ?? '')],
];
?>
<div class="card">
  <h1>Edit company</h1><div class="small"><?= Util::e($company['name'] ?? '') ?></div><hr />
  <?php if (!empty($error)): ?><div class="flash err"><?= Util::e($error) ?></div><?php endif; ?>

  <?php
    $action = '?r=company/edit&id='.(int)$company['id'];
    $submitLabel = 'Save';
    $cancelUrl = '?r=company/view&id='.(int)$company['id'];
    require __DIR__ . '/../partials/form.php';
  ?>
</div>
