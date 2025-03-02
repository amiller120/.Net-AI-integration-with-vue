<template>
  <div>
    <div class="chat-container">
      <div class="chat-box" :class="{ 'expanded': isExpanded }">
        <div class="chat-box-header">
          <div class="chat-box-header-text">Chat</div>
          <div class="chat-box-header-controls">
            <div class="chat-box-header-expand" @click="toggleExpand">
              <i :class="isExpanded ? 'fa fa-compress' : 'fa fa-expand'"></i>
            </div>
            <div class="chat-box-header-close" @click="closeChat">X</div>
          </div>
        </div>
        <div class="chat-box-body">
          <div class="chat-box-body-messages">
            <div v-for="(message, index) in messages" :key="index" class="chat-box-body-message"
              :class="{ 'sent': message.isUser, 'received': !message.isUser }">
              <div class="chat-box-body-message-avatar">
                <img :src="message.isUser ? userAvatar : botAvatar" alt="avatar" />
              </div>
              <div class="chat-box-body-message-text">
                <div class="chat-box-body-message-text-name">{{ message.isUser ? 'You' : 'AI Assistant' }}</div>
                <div class="chat-box-body-message-text-message">
                  {{ message.text }}
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="chat-box-footer">
          <input type="text" placeholder="Type a message..." v-model="newMessage" @keyup.enter="sendMessage" />
          <button @click="sendMessage">Send</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, nextTick } from 'vue';

interface ChatMessage {
  text: string;
  isUser: boolean;
  timestamp: Date;
}

const messages = ref<ChatMessage[]>([]);
const newMessage = ref('');
const userAvatar = 'https://via.placeholder.com/150?text=You';
const botAvatar = 'https://via.placeholder.com/150?text=AI';
const chatBoxBody = ref<HTMLElement | null>(null);
const isExpanded = ref(false);

const sendMessage = async () => {
  if (!newMessage.value.trim()) return;

  const userMessage: ChatMessage = {
    text: newMessage.value,
    isUser: true,
    timestamp: new Date()
  };
  messages.value.push(userMessage);

  // Clear input
  const messageToSend = newMessage.value;
  newMessage.value = '';

  // Scroll to bottom
  await nextTick();
  scrollToBottom();

  try {
    // Send to backend
    const response = await fetch(`https://localhost:5001/chat`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ message: messageToSend }),
    });

    if (!response.ok) {
      throw new Error('Network response was not ok');
    }
    const data = await response.json();

    // Add response to messages
    const botMessage: ChatMessage = {
      text: data.message.contents[0].text || "Sorry, I couldn't process your request.",
      isUser: false,
      timestamp: new Date()
    };
    messages.value.push(botMessage);

    // Scroll to bottom after adding new message
    await nextTick();
    scrollToBottom();

  } catch (error) {
    console.error('Error sending message:', error);

    // Add error message
    messages.value.push({
      text: "Sorry, there was an error processing your message.",
      isUser: false,
      timestamp: new Date()
    });
  }
};

// Scroll chat to bottom
const scrollToBottom = () => {
  const element = document.querySelector('.chat-box-body');
  if (element) {
    element.scrollTop = element.scrollHeight;
  }
};

// Toggle expanded state
const toggleExpand = () => {
  isExpanded.value = !isExpanded.value;
  nextTick(() => {
    scrollToBottom();
  });
};


const closeChat = () => {
  console.log('Chat closed');
};

onMounted(() => {
  // Initial welcome message
  messages.value.push({
    text: "Hello! How can I assist you today?",
    isUser: false,
    timestamp: new Date()
  });
});
</script>

<style>
.chat-container {
  position: fixed;
  bottom: 0;
  right: 0;
  margin: 20px;
}

.chat-box {
  width: 350px;
  height: 450px;
  background-color: #fff;
  border-radius: 10px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);
  display: flex;
  flex-direction: column;
  overflow: hidden;
  transition: width 0.3s ease, height 0.3s ease;
}

.chat-box.expanded {
  width: 500px;
  height: 600px;
}

.chat-box-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px;
  background-color: #4a6fa5;
  color: white;
  font-weight: bold;
}

.chat-box-header-controls {
  display: flex;
  align-items: center;
}

.chat-box-header-expand {
  cursor: pointer;
  font-size: 16px;
  margin-right: 15px;
  transition: color 0.2s;
}

.chat-box-header-expand:hover {
  color: #ddd;
}

.chat-box-header-close {
  cursor: pointer;
  font-size: 16px;
  transition: color 0.2s;
}

.chat-box-header-close:hover {
  color: #ff6b6b;
}

.chat-box-body {
  flex: 1;
  padding: 15px;
  overflow-y: auto;
  background-color: #f5f7fb;
}

.chat-box-body-messages {
  display: flex;
  flex-direction: column;
}

.chat-box-body-message {
  display: flex;
  margin-bottom: 15px;
}

.chat-box-body-message-avatar {
  margin-right: 10px;
}

.chat-box-body-message-avatar img {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  object-fit: cover;
}

.chat-box-body-message-text {
  max-width: 70%;
  background-color: #e6eaf0;
  padding: 10px;
  border-radius: 12px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.chat-box-body-message-text-name {
  font-size: 12px;
  font-weight: bold;
  margin-bottom: 5px;
  color: #4a6fa5;
}

.chat-box-body-message-text-message {
  font-size: 14px;
  color: #333;
  line-height: 1.4;
}

.chat-box-footer {
  display: flex;
  padding: 15px;
  background-color: #fff;
  border-top: 1px solid #e6eaf0;
}

.chat-box-footer input {
  flex: 1;
  padding: 10px 15px;
  border: 1px solid #ccc;
  border-radius: 20px;
  outline: none;
  font-size: 14px;
  margin-right: 10px;
}

.chat-box-footer input:focus {
  border-color: #4a6fa5;
}

.chat-box-footer button {
  background-color: #4a6fa5;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 20px;
  cursor: pointer;
  font-weight: bold;
  transition: background-color 0.2s;
}

.chat-box-footer button:hover {
  background-color: #3d5d8a;
}

.chat-box-body-message.received {
  justify-content: flex-start;
}

.chat-box-body-message.sent {
  flex-direction: row-reverse;
  justify-content: flex-end;
}

.chat-box-body-message.sent .chat-box-body-message-avatar {
  margin-right: 0;
  margin-left: 10px;
}

.chat-box-body-message.sent .chat-box-body-message-text {
  background-color: #4a6fa5;
  color: white;
}

.chat-box-body-message.sent .chat-box-body-message-text-name,
.chat-box-body-message.sent .chat-box-body-message-text-message {
  color: white;
}
</style>
