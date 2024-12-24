function animateTitle(elementId) {
    const container = document.getElementById(elementId);
    const text = container.textContent;
    container.textContent = "";

    text.split("").forEach((char, index) => {
        const span = document.createElement("span");
        span.textContent = char;
        span.className = "letter";
        container.appendChild(span);
        setTimeout(() => {
            letter.style.opacity = "1";
            letter.style.transform = "translateY(0) scale(1)";
        }, index * 200);
    });
}

animateTitle("title");