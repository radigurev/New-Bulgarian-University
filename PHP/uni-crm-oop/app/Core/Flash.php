<?php
declare(strict_types=1);

namespace App\Core;

final class Flash
{
    public function __construct(private Session $session) {}

    public function set(string $type, string $message): void
    {
        $this->session->set('flash', ['type' => $type, 'message' => $message]);
    }

    public function get(): ?array
    {
        $f = $this->session->get('flash');
        if (!$f) return null;
        $this->session->remove('flash');
        return $f;
    }
}
