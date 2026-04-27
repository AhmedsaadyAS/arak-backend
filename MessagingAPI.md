# Arak — Messaging Feature Documentation

> **Audience**: Flutter Android frontend team  
> **Backend**: ASP.NET Core 9 REST API  
> **Auth**: JWT Bearer Token  
> **Last updated**: 2026-04-27

---

## Table of Contents

1. [Overview](#overview)
2. [Authentication](#authentication)
3. [API Endpoints Reference](#api-endpoints-reference)
4. [Data Models](#data-models)
5. [Integration Guide](#integration-guide)
6. [Error Handling](#error-handling)
7. [Flutter Code Examples](#flutter-code-examples)

---

## Overview

The messaging feature enables **any authenticated user** (parent, teacher, admin, etc.) to send text messages to any other user in the system. Messages are organized into **conversations** — a conversation is the collection of all messages exchanged between two users.

### Architecture

```
┌──────────────────┐         HTTPS + JWT          ┌──────────────────────┐
│   Flutter App    │  ──────────────────────────►  │   ASP.NET Core API   │
│                  │  ◄──────────────────────────  │                      │
│  • Chat List     │       JSON Responses          │  ConversationsCtrl   │
│  • Chat Screen   │                               │  MessageService      │
│  • Send Message  │                               │  MessageRepository   │
└──────────────────┘                               │  SQL Server DB       │
                                                   └──────────────────────┘
```

### Key Concepts

| Concept | Description |
|---|---|
| **Conversation** | All messages between the current user and another user, identified by the other user's ID |
| **Participant** | The other user in a conversation (not the current user) |
| **Message** | A single text message with sender, receiver, content, and timestamps |
| **Read Status** | Determined by whether `readAt` is set (not null = read) |

---

## Authentication

All messaging endpoints require a valid **JWT Bearer token** in the `Authorization` header.

### Step 1 — Login to get a token

```
POST /api/Auth/login
Content-Type: application/json

{
  "email": "parent@arak.com",
  "password": "Parent@123"
}
```

**Response** (200 OK):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2026-04-28T10:00:00Z",
  "user": {
    "id": "abc-123-def",
    "name": "Ahmed Ali",
    "email": "parent@arak.com",
    "role": "Parent",
    "avatar": "AH",
    "roleId": 7
  }
}
```

### Step 2 — Use the token in all messaging requests

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

> **Important**: Store the `user.id` from the login response. You will need it to determine which messages were sent by the current user vs received.

---

## API Endpoints Reference

**Base URL**: `https://{your-server}/api`

All endpoints are under the `Conversations` controller.

---

### 1. List All Conversations

Retrieves all conversations for the currently logged-in user, sorted by most recent message first.

```
GET /api/Conversations
Authorization: Bearer {token}
```

**Response** (200 OK):
```json
[
  {
    "participantId": "user-id-456",
    "lastMessage": "See you tomorrow at the meeting",
    "lastMessageTime": "2026-04-27T10:30:00Z",
    "unreadCount": 3,
    "participant": {
      "name": "Ms. Sara Ahmed",
      "avatar": ""
    }
  },
  {
    "participantId": "user-id-789",
    "lastMessage": "Thank you for the update",
    "lastMessageTime": "2026-04-26T15:00:00Z",
    "unreadCount": 0,
    "participant": {
      "name": "Mr. Omar Khalid",
      "avatar": ""
    }
  }
]
```

| Field | Type | Description |
|---|---|---|
| `participantId` | `string` | The other user's ID — use this to open the chat and send messages |
| `lastMessage` | `string` | Text content of the most recent message in the conversation |
| `lastMessageTime` | `DateTime` (UTC) | When the most recent message was sent — use for "2 hours ago" display |
| `unreadCount` | `int` | Number of messages sent **to** the current user that haven't been read yet |
| `participant.name` | `string` | Display name of the other user |
| `participant.avatar` | `string` | Avatar URL (currently empty — use initials as fallback) |

---

### 2. Get Chat History (Paginated)

Retrieves messages between the current user and a specific user. Results are returned in **chronological order** (oldest first within the page) and are **paginated** — most recent messages are on page 1.

```
GET /api/Conversations/{userId}/messages?page=1&pageSize=50
Authorization: Bearer {token}
```

| Parameter | Type | Location | Default | Description |
|---|---|---|---|---|
| `userId` | `string` | URL path | *required* | The other user's ID (the `participantId` from the conversations list) |
| `page` | `int` | Query string | `1` | Page number (1 = most recent messages) |
| `pageSize` | `int` | Query string | `50` | Number of messages per page (max recommended: 100) |

**Response** (200 OK):
```json
[
  {
    "id": 42,
    "senderId": "current-user-id",
    "senderName": "Ahmed Ali",
    "receiverId": "user-id-456",
    "receiverName": "Ms. Sara Ahmed",
    "content": "Hello, how is my son doing in class?",
    "sentAt": "2026-04-27T09:00:00Z",
    "readAt": "2026-04-27T09:05:00Z",
    "isRead": true
  },
  {
    "id": 43,
    "senderId": "user-id-456",
    "senderName": "Ms. Sara Ahmed",
    "receiverId": "current-user-id",
    "receiverName": "Ahmed Ali",
    "content": "He is doing great! Very active in class.",
    "sentAt": "2026-04-27T09:15:00Z",
    "readAt": null,
    "isRead": false
  }
]
```

| Field | Type | Description |
|---|---|---|
| `id` | `int` | Unique message ID |
| `senderId` | `string` | User ID of who sent the message |
| `senderName` | `string` | Display name of the sender |
| `receiverId` | `string` | User ID of who received the message |
| `receiverName` | `string` | Display name of the receiver |
| `content` | `string` | Message text (max 4000 characters) |
| `sentAt` | `DateTime` (UTC) | When the message was sent |
| `readAt` | `DateTime?` (UTC) | When the message was read — `null` if unread |
| `isRead` | `bool` | `true` if the message has been read (computed from `readAt`) |

**Pagination behavior**:
- **Page 1** = the most recent 50 messages
- **Page 2** = the next 50 older messages
- **Empty array** `[]` = no more messages (stop loading)
- Messages within each page are in **chronological order** (oldest → newest)

---

### 3. Send a Message

Sends a new message to a specific user.

```
POST /api/Conversations/{userId}/messages
Authorization: Bearer {token}
Content-Type: application/json

{
  "content": "Hello, how is my son doing in class?"
}
```

| Parameter | Type | Location | Description |
|---|---|---|---|
| `userId` | `string` | URL path | The receiver's user ID |
| `content` | `string` | JSON body | Message text (**required**, max 4000 chars) |

**Response** (201 Created):
```json
{
  "id": 44,
  "senderId": "current-user-id",
  "senderName": "Ahmed Ali",
  "receiverId": "user-id-456",
  "receiverName": "Ms. Sara Ahmed",
  "content": "Hello, how is my son doing in class?",
  "sentAt": "2026-04-27T12:00:00Z",
  "readAt": null,
  "isRead": false
}
```

**Error responses**:

| Status | When | Body |
|---|---|---|
| `400` | `content` is missing, empty, or exceeds 4000 chars | Validation error details |
| `401` | Token is missing or invalid | — |
| `404` | The `userId` in the URL does not exist in the system | `"Receiver not found."` |

---

### 4. Mark a Single Message as Read

Marks one specific message as read. Only the **receiver** of a message can mark it as read.

```
PATCH /api/Conversations/{userId}/messages/{messageId}/read
Authorization: Bearer {token}
```

| Parameter | Type | Location | Description |
|---|---|---|---|
| `userId` | `string` | URL path | The other user's ID (conversation partner) |
| `messageId` | `int` | URL path | The ID of the message to mark as read |

**Response** (200 OK):
```json
{
  "success": true
}
```

**Error**: `404` if the message doesn't exist or the current user is not the receiver.

---

### 5. Mark Entire Conversation as Read (Batch)

Marks **all unread messages** in a conversation as read in one request. This is the **recommended** approach — call this when the user opens a chat screen.

```
PATCH /api/Conversations/{userId}/read
Authorization: Bearer {token}
```

| Parameter | Type | Location | Description |
|---|---|---|---|
| `userId` | `string` | URL path | The other user's ID (conversation partner) |

**Response** (200 OK):
```json
{
  "success": true,
  "markedCount": 5
}
```

| Field | Type | Description |
|---|---|---|
| `success` | `bool` | Always `true` on success |
| `markedCount` | `int` | Number of messages that were marked as read (0 if all were already read) |

---

## Data Models

### Dart Model Classes

Use these Dart classes to deserialize the API responses:

```dart
class Conversation {
  final String participantId;
  final String lastMessage;
  final DateTime lastMessageTime;
  final int unreadCount;
  final Participant participant;

  Conversation({
    required this.participantId,
    required this.lastMessage,
    required this.lastMessageTime,
    required this.unreadCount,
    required this.participant,
  });

  factory Conversation.fromJson(Map<String, dynamic> json) {
    return Conversation(
      participantId: json['participantId'],
      lastMessage: json['lastMessage'],
      lastMessageTime: DateTime.parse(json['lastMessageTime']),
      unreadCount: json['unreadCount'],
      participant: Participant.fromJson(json['participant']),
    );
  }
}

class Participant {
  final String name;
  final String avatar;

  Participant({required this.name, required this.avatar});

  factory Participant.fromJson(Map<String, dynamic> json) {
    return Participant(
      name: json['name'],
      avatar: json['avatar'] ?? '',
    );
  }

  /// Generate initials for avatar fallback (e.g., "Ms. Sara Ahmed" → "MS")
  String get initials {
    if (name.isEmpty) return '?';
    final parts = name.split(' ');
    if (parts.length >= 2) {
      return '${parts[0][0]}${parts[1][0]}'.toUpperCase();
    }
    return name[0].toUpperCase();
  }
}

class Message {
  final int id;
  final String senderId;
  final String senderName;
  final String receiverId;
  final String receiverName;
  final String content;
  final DateTime sentAt;
  final DateTime? readAt;
  final bool isRead;

  Message({
    required this.id,
    required this.senderId,
    required this.senderName,
    required this.receiverId,
    required this.receiverName,
    required this.content,
    required this.sentAt,
    this.readAt,
    required this.isRead,
  });

  factory Message.fromJson(Map<String, dynamic> json) {
    return Message(
      id: json['id'],
      senderId: json['senderId'],
      senderName: json['senderName'],
      receiverId: json['receiverId'],
      receiverName: json['receiverName'],
      content: json['content'],
      sentAt: DateTime.parse(json['sentAt']),
      readAt: json['readAt'] != null ? DateTime.parse(json['readAt']) : null,
      isRead: json['isRead'],
    );
  }

  /// Check if this message was sent by the current user
  bool isMine(String currentUserId) => senderId == currentUserId;
}
```

---

## Integration Guide

### Screen 1: Conversations List

This screen shows all conversations for the current user, similar to WhatsApp's main chat list.

```
┌─────────────────────────────────┐
│  Messages                       │
├─────────────────────────────────┤
│ ┌──┐ Ms. Sara Ahmed        3   │  ← unreadCount badge
│ │MS│ See you tomorrow at...     │  ← lastMessage (truncated)
│ └──┘ 10:30 AM                   │  ← lastMessageTime (formatted)
├─────────────────────────────────┤
│ ┌──┐ Mr. Omar Khalid           │
│ │MO│ Thank you for the update   │
│ └──┘ Yesterday                  │
└─────────────────────────────────┘
```

**Flow**:
1. Call `GET /api/Conversations` on screen load
2. Display each conversation with participant name, last message preview, time, and unread badge
3. On tap → navigate to Chat Screen with `participantId` and `participant.name`
4. On return from chat → refresh the list to update unread counts

### Screen 2: Chat Screen

This screen shows the message history with a specific user, with a text input at the bottom.

```
┌─────────────────────────────────┐
│ ← Ms. Sara Ahmed               │
├─────────────────────────────────┤
│                                 │
│        Hello, how is my  ─────┐ │  ← sent by me (right-aligned)
│        son doing?         9:00│ │
│                          ─────┘ │
│                                 │
│ ┌─────                          │  ← received (left-aligned)
│ │He is doing great!      9:15 │ │
│ │Very active in class.        │ │
│ └─────                    ✓✓  │ │  ← read status (✓✓ = read)
│                                 │
│  ▲ Load older messages          │  ← pagination trigger
├─────────────────────────────────┤
│ [Type a message...    ] [Send]  │
└─────────────────────────────────┘
```

**Flow**:
1. Call `GET /api/Conversations/{userId}/messages?page=1&pageSize=50` on screen load
2. Immediately call `PATCH /api/Conversations/{userId}/read` to mark all messages as read
3. Display messages — use `senderId == currentUserId` to determine alignment (left vs right)
4. On scroll up (to top) → load `page=2`, prepend older messages
5. On send → call `POST /api/Conversations/{userId}/messages` with the text
6. Append the returned message to the list locally (no need to re-fetch)

### Determining Message Direction

```dart
// In your chat screen widget:
final currentUserId = authProvider.userId; // from login response

for (final msg in messages) {
  if (msg.isMine(currentUserId)) {
    // → Right-aligned bubble (sent by me)
    // Show single ✓ if !isRead, double ✓✓ if isRead
  } else {
    // ← Left-aligned bubble (received)
  }
}
```

### Read Status Icons

| Condition | Icon | Meaning |
|---|---|---|
| Message sent by me, `isRead == false` | ✓ | Delivered but not read |
| Message sent by me, `isRead == true` | ✓✓ | Read by the other user |
| Message received by me | (no icon) | — |

### Pagination — "Load More" Pattern

```dart
int _currentPage = 1;
bool _hasMore = true;
List<Message> _messages = [];

Future<void> loadMessages(String otherUserId) async {
  final newMessages = await api.getConversationMessages(
    otherUserId, 
    page: _currentPage, 
    pageSize: 50,
  );
  
  if (newMessages.isEmpty) {
    _hasMore = false; // No more pages
    return;
  }
  
  // Prepend older messages (page 1 = newest, page 2 = older)
  _messages = [...newMessages, ..._messages];
  _currentPage++;
}
```

---

## Error Handling

All errors follow standard HTTP status codes:

| Status | Meaning | Action |
|---|---|---|
| `200` | Success | Process the response |
| `201` | Created (message sent) | Process the response |
| `400` | Validation error (empty content, etc.) | Show the error message to the user |
| `401` | Token expired or invalid | Redirect to login screen |
| `404` | User not found / message not found | Show "User not found" or "Message not found" |
| `500` | Server error | Show generic error, retry later |

### Handling 401 Globally

Set up an HTTP interceptor to catch `401` responses and redirect to login:

```dart
class AuthInterceptor extends Interceptor {
  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    if (err.response?.statusCode == 401) {
      // Token expired — navigate to login
      navigatorKey.currentState?.pushReplacementNamed('/login');
    }
    super.onError(err, handler);
  }
}
```

---

## Flutter Code Examples

### API Service Class

```dart
import 'package:dio/dio.dart';

class MessagingApi {
  final Dio _dio;
  
  MessagingApi(this._dio);
  
  /// GET /api/Conversations
  Future<List<Conversation>> getConversations() async {
    final response = await _dio.get('/api/Conversations');
    return (response.data as List)
        .map((json) => Conversation.fromJson(json))
        .toList();
  }
  
  /// GET /api/Conversations/{userId}/messages
  Future<List<Message>> getConversationMessages(
    String userId, {
    int page = 1,
    int pageSize = 50,
  }) async {
    final response = await _dio.get(
      '/api/Conversations/$userId/messages',
      queryParameters: {'page': page, 'pageSize': pageSize},
    );
    return (response.data as List)
        .map((json) => Message.fromJson(json))
        .toList();
  }
  
  /// POST /api/Conversations/{userId}/messages
  Future<Message> sendMessage(String userId, String content) async {
    final response = await _dio.post(
      '/api/Conversations/$userId/messages',
      data: {'content': content},
    );
    return Message.fromJson(response.data);
  }
  
  /// PATCH /api/Conversations/{userId}/read
  Future<void> markConversationAsRead(String userId) async {
    await _dio.patch('/api/Conversations/$userId/read');
  }
  
  /// PATCH /api/Conversations/{userId}/messages/{messageId}/read
  Future<void> markMessageAsRead(String userId, int messageId) async {
    await _dio.patch(
      '/api/Conversations/$userId/messages/$messageId/read',
    );
  }
}
```

### Dio Setup with Auth

```dart
final dio = Dio(BaseOptions(
  baseUrl: 'https://your-server.com',
  headers: {
    'Content-Type': 'application/json',
  },
));

// Add the JWT token after login
void setAuthToken(String token) {
  dio.options.headers['Authorization'] = 'Bearer $token';
}
```

---

## Quick Reference Card

```
┌──────────────────────────────────────────────────────────────────────┐
│                     MESSAGING API — QUICK REFERENCE                  │
├────────┬─────────────────────────────────────────────┬───────────────┤
│ Method │ Endpoint                                    │ Purpose       │
├────────┼─────────────────────────────────────────────┼───────────────┤
│ GET    │ /api/Conversations                          │ List chats    │
│ GET    │ /api/Conversations/{userId}/messages        │ Chat history  │
│ POST   │ /api/Conversations/{userId}/messages        │ Send message  │
│ PATCH  │ /api/Conversations/{userId}/messages/{id}/read │ Read one   │
│ PATCH  │ /api/Conversations/{userId}/read            │ Read all      │
├────────┴─────────────────────────────────────────────┴───────────────┤
│ Auth: Bearer {JWT token from POST /api/Auth/login}                   │
│ Content-Type: application/json                                       │
│ All timestamps are UTC — convert to local time for display           │
│ Pagination: ?page=1&pageSize=50 (page 1 = most recent)              │
└──────────────────────────────────────────────────────────────────────┘
```
