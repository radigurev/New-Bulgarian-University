<?php
declare(strict_types=1);

/**
 * Expected variables:
 * - $headers: string[]
 * - $rows: array<array<string>>  (cells are HTML-ready strings)
 * - $emptyText: string (optional)
 */
$emptyText = $emptyText ?? 'No data.';
$headers = $headers ?? [];
$rows = $rows ?? [];
?>
<table>
  <thead>
    <tr>
      <?php foreach ($headers as $h): ?>
        <th><?= $h ?></th>
      <?php endforeach; ?>
    </tr>
  </thead>
  <tbody>
    <?php if (!$rows): ?>
      <tr><td colspan="<?= (int)max(1, count($headers)) ?>" class="small"><?= $emptyText ?></td></tr>
    <?php else: ?>
      <?php foreach ($rows as $r): ?>
        <tr>
          <?php foreach ($r as $cell): ?>
            <td><?= $cell ?></td>
          <?php endforeach; ?>
        </tr>
      <?php endforeach; ?>
    <?php endif; ?>
  </tbody>
</table>
