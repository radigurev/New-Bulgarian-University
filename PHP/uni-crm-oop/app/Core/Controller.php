<?php
declare(strict_types=1);

namespace App\Core;

use App\Services\AuthService;

abstract class Controller
{
    protected View $view;
    protected Session $session;
    protected Flash $flash;
    protected Csrf $csrf;
    protected AuthService $auth;

    public function __construct(protected Container $c)
    {
        $this->view = $c->get(View::class);
        $this->session = $c->get(Session::class);
        $this->flash = $c->get(Flash::class);
        $this->csrf = $c->get(Csrf::class);
        $this->auth = $c->get(AuthService::class);
    }

    protected function requireLogin(): void
    {
        if (!$this->auth->isLoggedIn()) {
            Util::redirect('auth/login');
        }
    }

    protected function render(string $template, array $data = []): void
    {
        $data['flash'] = $this->flash->get();
        $data['currentUser'] = $this->auth->currentUser();
        $data['csrfToken'] = $this->csrf->token();
        $this->view->render($template, $data);
    }
}
