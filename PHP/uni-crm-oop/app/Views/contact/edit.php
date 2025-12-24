<?php
declare(strict_types=1);

use App\Core\Util;

$fields = [
  ['name'=>'first_name','label'=>'First name','type'=>'text','required'=>true,'value'=>($_POST['first_name'] ?? $contact['first_name'] ?? '')],
  ['name'=>'last_name','label'=>'Last name','type'=>'text','required'=>true,'value'=>($_POST['last_name'] ?? $contact['last_name'] ?? '')],
  ['name'=>'position','label'=>'Position','type'=>'text','value'=>($_POST['position'] ?? $contact['position'] ?? '')],
  ['name'=>'email','label'=>'Email','type'=>'email','value'=>($_POST['email'] ?? $contact['email'] ?? '')],
  ['name'=>'phone','label'=>'Phone','type'=>'text','value'=>($_POST['phone'] ?? $contact['phone'] ?? '')],
];
?>
<div class="card">
  <h1>Edit contact</h1>
  <div class="small">
    <?= Util::e(($contact['first_name'] ?? '').' '.($contact['last_name'] ?? '')) ?>
    • <?= Util::e($contact['company_name'] ?? '') ?>
  </div>
  <hr />
  <?php if (!empty($error)): ?><div class="flash err"><?= Util::e($error) ?></div><?php endif; ?>

  <?php
    $action = '?r=contact/edit&id='.(int)$contact['id'];
    $submitLabel = 'Save';
    $cancelUrl = '?r=company/view&id='.(int)$contact['company_id'];
    require __DIR__ . '/../partials/form.php';
  ?>
</div>
