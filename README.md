````md
# 🧠 JobMasterApi

**JobMasterApi** is a secure and intelligent resume management and job application assistant API. It enables users to upload resumes, analyze job fit, and generate tailored cover letters using GPT-powered insights.

---

## 🚀 Features

- ✅ **JWT Authentication**
- 📄 **Resume Upload & Management**
- 🧠 **Job Fit Analysis** using OpenAI GPT
- ✍️ **Cover Letter Generation**
- 📥 **Download Cover Letter as DOCX**
- 🔐 Secured endpoints with ASP.NET Identity
- 📦 Clean RESTful API design with Swagger documentation

---

## 📦 Tech Stack

- .NET 9 (Minimal APIs + Controllers)
- ASP.NET Identity
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- OpenAI GPT (via `IGptService`)
- Swagger / OpenAPI

---

## 🛠️ Setup Instructions

### 1. Clone the repo

```bash
git clone https://github.com/felixkpt/JobMasterApi.git
cd JobMasterApi
````

### 2. Configure `appsettings.json`

```json
{
  "Jwt": {
    "Key": "SuperSecretKey123456!",
    "Issuer": "JobMasterApi",
    "Audience": "JobMasterApiUsers",
    "ExpiresInMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=JobMasterDb;User Id=sa;Password=YourPassword123;"
  },
	"OpenAI": {
		"ApiKey": "sk-"
	}
}
```

### 3. Run Migrations

```bash
dotnet ef database update
```

### 4. Run the API

```bash
dotnet run
```

Then visit: [http://localhost:5120/swagger](http://localhost:5120/swagger)

---

## 🔑 Authentication

### Register

```
POST /api/v1/auth/register
```

### Login

```
POST /api/v1/auth/login
```

Returns a JWT token. Include it in the `Authorization` header:

```
Authorization: Bearer <token>
```

---

## 📄 Resume API

### Upload Resume

```
POST /api/v1/resumes
Content-Type: multipart/form-data
Authorization: Bearer <token>
```

### Get My Resumes

```
GET /api/v1/resumes
Authorization: Bearer <token>
```

### Delete Resume

```
DELETE /api/v1/resumes/{resumeId}
Authorization: Bearer <token>
```

---

## 🧠 Application Wizard

### Analyze Job Fit

```
POST /api/v1/application-wizard/analyze-fit
Authorization: Bearer <token>
Body:
{
  "resumeId": "guid",
  "jobDescription": "string"
}
```

### Generate Cover Letter

```
POST /api/v1/application-wizard/generate-cover-letter
Authorization: Bearer <token>
Body:
{
  "resumeId": "guid",
  "jobDescription": "string"
}
```

### Download Cover Letter as DOCX

```
GET /api/v1/application-wizard/download-cover-letter-docx?coverLetter=...
Authorization: Bearer <token>
```

---

## 🧪 Testing

* Swagger UI available at `/swagger`
* Use Postman or Swagger to test all secured endpoints
* Token-based access enforced with `[Authorize]` attributes

---

## 👥 Contributing

Feel free to fork and contribute! For major changes, open an issue first to discuss what you'd like to change.

---

## 📄 License

MIT

---

## ✨ Author

**JobMasterApi** – developed by [Kiptoo Kipkiro (Felix)](https://github.com/felixkpt)

```