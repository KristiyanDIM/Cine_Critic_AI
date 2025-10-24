    document.getElementById("chatbot-button").addEventListener("click", function () {
        document.getElementById("chatbot-window").classList.toggle("hidden");
});

    document.getElementById("chat-close").addEventListener("click", function () {
        document.getElementById("chatbot-window").classList.add("hidden");
});

    document.getElementById("chat-send").addEventListener("click", sendMessage);
    document.getElementById("chat-input").addEventListener("keypress", function (e) {
    if (e.key === "Enter") sendMessage();
});

    function sendMessage() {
    const input = document.getElementById("chat-input");
    const message = input.value.trim();
    if (message === "") return;

    const messages = document.getElementById("chat-messages");

    const userMsg = document.createElement("div");
    userMsg.classList.add("user-message");
    userMsg.textContent = message;
    messages.appendChild(userMsg);

    input.value = "";

    // Simple fake bot reply
    setTimeout(() => {
        const botMsg = document.createElement("div");
    botMsg.classList.add("bot-message");
    botMsg.textContent = "🤖 Много интересен въпрос! Скоро ще добавим реален AI чат.";
    messages.appendChild(botMsg);
    messages.scrollTop = messages.scrollHeight;
    }, 600);

    messages.scrollTop = messages.scrollHeight;
}
