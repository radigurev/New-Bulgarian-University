<?php
declare(strict_types=1);

use App\Core\Util;
?>
<div class="container" style="max-width:520px;">
  <div class="card">
    <h1>Login</h1>
    <div class="helper">
      First time? Open <a href="?r=auth/initAdmin">Init admin</a> (creates admin@uni.local / admin123)
    </div>
    <hr />
    <?php if (!empty($error)): ?><div class="flash err"><?= Util::e($error) ?></div><?php endif; ?>
    <form method="post" action="?r=auth/login">
      <input type="hidden" name="csrf" value="<?= Util::e($csrfToken) ?>" />
      <label>Email</label>
      <input name="email" type="email" required />
      <label>Password</label>
      <input name="password" type="password" required />
      <div class="actions" style="margin-top:12px;">
        <button class="btn" type="submit">Login</button>
      </div>
    </form>
  </div>
</div>
