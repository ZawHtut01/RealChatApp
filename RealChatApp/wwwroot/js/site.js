document.getElementById("chatForm").addEventListener("submit", function (e) {
    e.preventDefault();
    const message = document.getElementById("messageInput").value.trim();
    if (!message) return;

    // Append message locally first
    appendMessage(currentUserId, receiverId, message, "You", currentUserProfile);

    // Send to server
    fetch("/Chat/SendMessage", {
        method: "POST",
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ receiverId: parseInt(receiverId), message })
    });

    // Send via SignalR
    connection.invoke("SendMessage", parseInt(currentUserId), parseInt(receiverId), message)
        .catch(err => console.error(err));

    // Clear input
    document.getElementById("messageInput").value = "";
});

function appendMessage(senderId, receiverId, message, senderName, profileImage) {
    const chatWindow = document.getElementById("chatWindow");
    const msgDiv = document.createElement("div");
    msgDiv.className = senderId == currentUserId ? "text-end mb-2" : "text-start mb-2";
    msgDiv.innerHTML = `
        <img src="${profileImage}" width="30" height="30" style="object-fit: cover; border-radius: 50%; vertical-align: middle;" />
        <strong>${senderName}</strong><br/>
        <span>${message}</span>
    `;
    chatWindow.appendChild(msgDiv);
    chatWindow.scrollTop = chatWindow.scrollHeight;
}



//document.addEventListener("DOMContentLoaded", function () {
//    const sendButton = document.getElementById("sendButton");
//    const messageInput = document.getElementById("messageInput");
//    const receiverIdInput = document.getElementById("receiverId");
//    const chatWindow = document.getElementById("chatWindow");

//    const currentUserId = '@currentUserId'; // You might want to pass this dynamically from your razor view
//    const receiverId = receiverIdInput.value;

//    // Setup SignalR connection
//    const connection = new signalR.HubConnectionBuilder()
//        .withUrl("/chathub")
//        .build();

//    connection.start()
//        .then(() => console.log("Connected to chat hub"))
//        .catch(err => console.error("SignalR connection error:", err));

//    // Receive message via SignalR and append to chat window
//    connection.on("ReceiveMessage", (senderId, rId, message, senderUsername, senderProfile) => {
//        // Only show message if between current user and receiver
//        if ((senderId == currentUserId && rId == receiverId) || (senderId == receiverId && rId == currentUserId)) {
//            const msgDiv = document.createElement("div");
//            msgDiv.className = senderId == currentUserId ? "text-end mb-2" : "text-start mb-2";

//            // Show profile image, username and message nicely formatted
//            msgDiv.innerHTML = `
//                <img src="${senderProfile}" width="30" height="30" style="object-fit: cover; border-radius: 50%; vertical-align: middle;" />
//                <strong>${senderId == currentUserId ? "You" : senderUsername}</strong><br/>
//                <span>${message}</span>
//            `;
//            chatWindow.appendChild(msgDiv);
//            chatWindow.scrollTop = chatWindow.scrollHeight; // scroll to bottom
//        }
//    });

//    // Handle form submission
//    document.getElementById("chatForm").addEventListener("submit", function (e) {
//        e.preventDefault();

//        const message = messageInput.value.trim();
//        if (!message) {
//            alert("Please enter a message.");
//            return;
//        }

//        // Send message to controller API
//        fetch("/Chat/SendMessage", {
//            method: "POST",
//            headers: { 'Content-Type': 'application/json' },
//            body: JSON.stringify({ receiverId: parseInt(receiverId), message: message })
//        })
//            .then(response => {
//                if (!response.ok) throw new Error("Failed to send message");
//                return response.json();
//            })
//            .then(() => {
//                // Notify SignalR hub about the new message
//                connection.invoke("SendMessage", parseInt(currentUserId), parseInt(receiverId), message)
//                    .catch(err => console.error("SignalR send error:", err));

//                messageInput.value = "";
//            })
//            .catch(err => alert(err.message));
//    });
//});
