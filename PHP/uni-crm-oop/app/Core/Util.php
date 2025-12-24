<?php
declare(strict_types=1);

namespace App\Core;

final class Util
{
    public static function e(?string $v): string
    {
        return htmlspecialchars((string)$v, ENT_QUOTES, 'UTF-8');
    }

    public static function redirect(string $route): void
    {
        header('Location: ?r=' . $route);
        exit;
    }

    public static function int(?string $v): ?int
    {
        if ($v === null || $v === '') return null;
        $x = filter_var($v, FILTER_VALIDATE_INT);
        return $x === false ? null : (int)$x;
    }
}
