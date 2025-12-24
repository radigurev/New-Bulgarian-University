<?php
declare(strict_types=1);

namespace App\Core;

final class View
{
    public function __construct(private string $viewsPath) {}

    public function render(string $template, array $data = []): void
    {
        $templateFile = rtrim($this->viewsPath, '/\\') . '/' . $template . '.php';
        if (!is_file($templateFile)) {
            throw new \RuntimeException("View not found: $templateFile");
        }

        extract($data, EXTR_SKIP);

        $layout = rtrim($this->viewsPath, '/\\') . '/layout.php';
        $contentFile = $templateFile;

        if (is_file($layout)) {
            require $layout;
        } else {
            require $templateFile;
        }
    }
}
