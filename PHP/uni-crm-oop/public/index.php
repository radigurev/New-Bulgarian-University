<?php
declare(strict_types=1);

require_once __DIR__ . '/../app/Core/Autoloader.php';
\App\Core\Autoloader::register();

$config = require __DIR__ . '/../app/Config/config.php';

$container = new \App\Core\Container($config);
$container->get(\App\Core\Session::class)->start();

$route = (string)($_GET['r'] ?? 'dashboard/index');
$route = trim($route);

[$controllerName, $actionName] = array_pad(explode('/', $route, 2), 2, 'index');
$controllerClass = '\\App\\Controllers\\' . ucfirst($controllerName) . 'Controller';
$actionMethod = $actionName . 'Action';

if (!class_exists($controllerClass)) {
    http_response_code(404);
    echo "Controller not found";
    exit;
}

$controller = new $controllerClass($container);

if (!method_exists($controller, $actionMethod)) {
    http_response_code(404);
    echo "Action not found";
    exit;
}

$controller->$actionMethod();
