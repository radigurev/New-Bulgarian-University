<?php
declare(strict_types=1);

use App\Core\Util;

$fields = [
  ['name'=>'name','label'=>'Name','type'=>'text','required'=>true,'value'=>$_POST['name'] ?? ''],
  ['name'=>'vat_number','label'=>'VAT','type'=>'text','value'=>$_POST['vat_number'] ?? ''],
  ['name'=>'email','label'=>'Email','type'=>'email','value'=>$_POST['email'] ?? ''],
  ['name'=>'phone','label'=>'Phone','type'=>'text','value'=>$_POST['phone'] ?? ''],
  ['name'=>'website','label'=>'Website','type'=>'text','value'=>$_POST['website'] ?? ''],
  ['name'=>'address','label'=>'Address','type'=>'text','value'=>$_POST['address'] ?? ''],
];
?>
<div class="card">
  <h1>New company</h1><hr />
  <?php if (!empty($error)): ?><div class="flash err"><?= Util::e($error) ?></div><?php endif; ?>

  <?php
    $action = '?r=company/create';
    $submitLabel = 'Create';
    $cancelUrl = '?r=company/index';
    require __DIR__ . '/../partials/form.php';
  ?>
</div>
