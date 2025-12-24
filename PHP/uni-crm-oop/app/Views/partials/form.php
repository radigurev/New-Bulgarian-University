<?php
declare(strict_types=1);

use App\Core\Util;

/**
 * Generic form renderer to reduce duplication.
 *
 * Expected variables:
 * - $action: string (full URL, e.g. "?r=company/create")
 * - $fields: array of field definitions:
 *     [
 *       'name' => 'title',
 *       'label' => 'Title',
 *       'type' => 'text'|'email'|'date'|'select'|'textarea'|'number',
 *       'value' => mixed,
 *       'required' => bool,
 *       'placeholder' => string,
 *       'help' => string,
 *       'options' => array of ['value'=>..., 'label'=>...]  (for select)
 *       'disabled' => bool
 *     ]
 * - $csrfToken: string
 * - $hidden: array of ['name'=>..., 'value'=>...]
 * - $submitLabel: string
 * - $cancelUrl: string (full URL) optional
 * - $columns: int 1 or 2 (default 2)
 */
$fields = $fields ?? [];
$hidden = $hidden ?? [];
$submitLabel = $submitLabel ?? 'Save';
$columns = (int)($columns ?? 2);
if ($columns < 1) $columns = 1;
?>
<form method="post" action="<?= Util::e($action) ?>">
  <input type="hidden" name="csrf" value="<?= Util::e($csrfToken) ?>" />
  <?php foreach ($hidden as $h): ?>
    <input type="hidden" name="<?= Util::e($h['name']) ?>" value="<?= Util::e((string)$h['value']) ?>" />
  <?php endforeach; ?>

  <?php if ($columns === 2): ?><div class="row"><?php endif; ?>

  <?php
  $i = 0;
  foreach ($fields as $f):
    $i++;
    $type = $f['type'] ?? 'text';
    $name = (string)($f['name'] ?? '');
    $label = (string)($f['label'] ?? $name);
    $value = $f['value'] ?? '';
    $required = !empty($f['required']);
    $placeholder = (string)($f['placeholder'] ?? '');
    $help = (string)($f['help'] ?? '');
    $disabled = !empty($f['disabled']);
  ?>
    <?php if ($columns === 2): ?><div><?php endif; ?>
      <label><?= Util::e($label) ?><?= $required ? ' *' : '' ?></label>

      <?php if ($type === 'textarea'): ?>
        <textarea name="<?= Util::e($name) ?>" <?= $required ? 'required' : '' ?> <?= $disabled ? 'disabled' : '' ?> placeholder="<?= Util::e($placeholder) ?>"><?= Util::e((string)$value) ?></textarea>
      <?php elseif ($type === 'select'): ?>
        <select name="<?= Util::e($name) ?>" <?= $required ? 'required' : '' ?> <?= $disabled ? 'disabled' : '' ?>>
          <?php
          $opts = $f['options'] ?? [];
          $strVal = (string)$value;
          foreach ($opts as $opt) {
              $ov = (string)($opt['value'] ?? '');
              $ol = (string)($opt['label'] ?? $ov);
              $sel = ($ov !== '' && $ov === $strVal) ? 'selected' : '';
              echo '<option value="' . Util::e($ov) . '" ' . $sel . '>' . Util::e($ol) . '</option>';
          }
          ?>
        </select>
      <?php else: ?>
        <input
          type="<?= Util::e($type) ?>"
          name="<?= Util::e($name) ?>"
          value="<?= Util::e((string)$value) ?>"
          <?= $required ? 'required' : '' ?>
          <?= $disabled ? 'disabled' : '' ?>
          placeholder="<?= Util::e($placeholder) ?>"
        />
      <?php endif; ?>

      <?php if ($help !== ''): ?><div class="helper"><?= Util::e($help) ?></div><?php endif; ?>
    <?php if ($columns === 2): ?></div><?php endif; ?>

    <?php if ($columns === 2 && $i % 2 === 0 && $i !== count($fields)): ?>
      </div><div class="row">
    <?php endif; ?>
  <?php endforeach; ?>

  <?php if ($columns === 2): ?></div><?php endif; ?>

  <div class="actions" style="margin-top:12px;">
    <button class="btn" type="submit"><?= Util::e($submitLabel) ?></button>
    <?php if (!empty($cancelUrl)): ?>
      <a class="btn light" href="<?= Util::e($cancelUrl) ?>">Cancel</a>
    <?php endif; ?>
  </div>
</form>
