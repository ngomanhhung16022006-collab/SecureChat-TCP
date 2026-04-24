 🔐 Secure Chat Application (AES)
 📌 Giới thiệu
Đây là một ứng dụng chat đơn giản được xây dựng theo mô hình **Client-Server** sử dụng **TCP Socket**.  
Hệ thống cho phép nhiều client kết nối tới server và trao đổi tin nhắn theo thời gian thực.

Điểm nổi bật của hệ thống là sử dụng **thuật toán AES** để mã hóa nội dung tin nhắn nhằm đảm bảo tính bảo mật khi truyền dữ liệu qua mạng.

---

 Tính năng chính

- 💬 Chat thời gian thực giữa nhiều client
- 👥 Hiển thị danh sách người dùng online
- 🔐 Mã hóa tin nhắn bằng AES
- 📡 Giao tiếp qua TCP
- 🧠 Sử dụng protocol riêng để xử lý dữ liệu
- ⚡ Xử lý đa luồng (Thread) để nhận dữ liệu liên tục

---

 🏗️ Kiến trúc hệ thống
 🖥️ Server
- Lắng nghe kết nối từ client
- Quản lý danh sách người dùng
- Broadcast tin nhắn tới các client
- Không giải mã nội dung (chỉ chuyển tiếp dữ liệu)

 💻 Client
- Kết nối tới server
- Gửi và nhận tin nhắn
- Mã hóa dữ liệu trước khi gửi (AES)
- Giải mã dữ liệu khi nhận

---
 🔐 Bảo mật (AES)

- Thuật toán sử dụng: AES (Advanced Encryption Standard)
- Loại: Symmetric Encryption (mã hóa đối xứng)
- Key và IV được sử dụng để mã hóa và giải mã

### 🔄 Quy trình:
