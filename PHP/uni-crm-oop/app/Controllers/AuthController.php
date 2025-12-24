<?php
declare(strict_types=1);

namespace App\Controllers;

use App\Core\Controller;
use App\Core\Util;

final class AuthController extends Controller
{
    public function loginAction(): void
    {
        if ($this->auth->isLoggedIn()) Util::redirect('dashboard/index');

        $error = '';
        if ($_SERVER['REQUEST_METHOD'] === 'POST') {
            $this->csrf->validate($_POST['csrf'] ?? null);
            $email = (string)($_POST['email'] ?? '');
            $pass  = (string)($_POST['password'] ?? '');

            if ($this->auth->login($email, $pass)) {
                $this->flash->set('ok', 'Welcome!');
                Util::redirect('dashboard/index');
            }
            $error = 'Invalid credentials.';
        }

        $this->render('auth/login', ['error' => $error]);
    }

    public function logoutAction(): void
    {
        $this->auth->logout();
        Util::redirect('auth/login');
    }

    public function initAdminAction(): void
    {
        $this->auth->initAdmin();
        $this->flash->set('ok', 'Admin ready: admin@uni.local / admin123');
        Util::redirect('auth/login');
    }
}
