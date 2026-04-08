# 📊 Portfolio Database Documentation (skills.md)

## 🧠 Overview

This database is designed for a **Portfolio System** using PostgreSQL (Supabase).

* All business logic is implemented using **PostgreSQL FUNCTIONS (not procedures)**
* All functions are called using:

```sql
SELECT * FROM functionName(...)
```

---

# ⚠️ IMPORTANT RULES

## 🔴 PostgreSQL Behavior

* Functions ≠ Procedures
* ALL operations use `CREATE FUNCTION`
* NEVER use `CALL`
* ALWAYS use `SELECT`

---

## 🔐 Security Rules

* All functions use parameters (no dynamic SQL)
* Safe from SQL Injection
* Strict typing enforced

---

# 🧱 DATABASE SCHEMA

---

## 👤 Persons

| Column      | Type    | Nullable |
| ----------- | ------- | -------- |
| ID          | bigint  | NO       |
| Name        | varchar | NO       |
| Description | text    | YES      |
| ImageUrl    | varchar | YES      |

---

## 🧾 About

| Column      | Type   |
| ----------- | ------ |
| ID          | bigint |
| Summary     | text   |
| FocusArea   | text   |
| Highlighted | text   |
| PersonID    | bigint |

---

## 📞 ContactInfo

| Column   | Type    |
| -------- | ------- |
| ID       | bigint  |
| Email    | varchar |
| Phone    | varchar |
| Location | varchar |
| PersonID | bigint  |

---

## 💼 Experiences

| Column      | Type            |
| ----------- | --------------- |
| ID          | bigint          |
| Name        | varchar         |
| Description | text            |
| CompanyName | varchar         |
| StartDate   | date            |
| EndDate     | date (nullable) |
| PersonID    | bigint          |

---

## 🏷 JobTitles

| Column   | Type    |
| -------- | ------- |
| ID       | bigint  |
| Title    | varchar |
| IsActive | boolean |
| PersonID | bigint  |

---

## 🧠 Skills

| Column          | Type    |
| --------------- | ------- |
| ID              | bigint  |
| Name            | varchar |
| Icon            | text    |
| SkillCategoryID | bigint  |

---

## 🗂 SkillCategories

| Column | Type    |
| ------ | ------- |
| ID     | bigint  |
| Name   | varchar |

---

## 🔗 PersonSkills

| Column   | Type   |
| -------- | ------ |
| ID       | bigint |
| PersonID | bigint |
| SkillID  | bigint |

---

## 🔗 ProjectSkills

| Column    | Type   |
| --------- | ------ |
| ID        | bigint |
| ProjectID | bigint |
| SkillID   | bigint |

---

## 🚀 Projects

| Column      | Type    |
| ----------- | ------- |
| ID          | bigint  |
| Title       | varchar |
| Description | text    |
| Details     | text    |
| ImageUrl    | text    |
| GitHubUrl   | text    |
| LiveUrl     | text    |
| PersonID    | bigint  |

---

## 🌐 SocialLinks

| Column   | Type    |
| -------- | ------- |
| ID       | bigint  |
| Platform | varchar |
| Url      | varchar |
| Icon     | text    |
| PersonID | bigint  |

---

## 👤 Users

| Column      | Type    |
| ----------- | ------- |
| id          | bigint  |
| Username    | varchar |
| Password    | varchar |
| IsActive    | boolean |
| Permissions | bigint  |
| PersonID    | bigint  |

---

# 🔗 RELATIONSHIPS

* Persons → About (1:1 / 1:N)

* Persons → ContactInfo (1:N)

* Persons → Experiences (1:N)

* Persons → JobTitles (1:N)

* Persons → Projects (1:N)

* Persons → SocialLinks (1:N)

* Persons → Users (1:1)

* Skills → SkillCategories (N:1)

* PersonSkills:

  * PersonID → Persons.ID
  * SkillID → Skills.ID

* ProjectSkills:

  * ProjectID → Projects.ID
  * SkillID → Skills.ID

---

# ⚙️ STORED FUNCTIONS (ALL)

---

## 👤 Persons

* getAllPersons()
* getPersonById(p_id)
* addNewPerson(p_name, p_description, p_imageurl)
* updatePersonById(p_id, ...)
* deletePersonById(p_id)

---

## 🧾 About

* getAllAbout()
* getAboutById(p_id)
* getAboutByPerson(p_personid)
* addNewAbout(...)
* updateAboutById(...)
* deleteAboutById(p_id)

---

## 📞 ContactInfo

* getAllContactInfo()
* getContactInfoById(p_id)
* getContactInfoByPerson(p_personid)
* addNewContactInfo(...)
* updateContactInfoById(...)
* deleteContactInfoById(p_id)

---

## 💼 Experiences

* getAllExperiences()
* getExperienceById(p_id)
* getExperiencesByPerson(p_personid)
* addNewExperience(...)
* updateExperienceById(...)
* deleteExperienceById(p_id)

---

## 🏷 JobTitles

* getAllJobTitles()
* getJobTitleById(p_id)
* getJobTitlesByPerson(p_personid)
* addNewJobTitle(...)
* updateJobTitleById(...)
* deleteJobTitleById(p_id)

---

## 🧠 Skills

* getAllSkills()
* getSkillById(p_id)
* addNewSkill(...)
* updateSkillById(...)
* deleteSkillById(p_id)

---

## 🗂 SkillCategories

* getAllSkillCategories()
* getSkillCategoryById(p_id)
* addNewSkillCategory(p_name)
* updateSkillCategoryById(p_id, p_name)
* deleteSkillCategoryById(p_id)

---

## 🔗 PersonSkills

* getAllPersonSkills()
* getPersonSkillById(p_id)
* getPersonSkillsByPerson(p_personid)
* addNewPersonSkill(p_personid, p_skillid)
* updatePersonSkillById(...)
* deletePersonSkillById(p_id)

---

## 🔗 ProjectSkills

* getAllProjectSkills()
* getProjectSkillById(p_id)
* addNewProjectSkill(p_projectid, p_skillid)
* updateProjectSkillById(...)
* deleteProjectSkillById(p_id)

---

## 🚀 Projects

* getAllProjects()
* getProjectById(p_id)
* getProjectsByPerson(p_personid)
* addNewProject(...)
* updateProjectById(...)
* deleteProjectById(p_id)

---

## 🌐 SocialLinks

* getAllSocialLinks()
* getSocialLinkById(p_id)
* getSocialLinksByPerson(p_personid)
* addNewSocialLink(...)
* updateSocialLinkById(...)
* deleteSocialLinkById(p_id)

---

## 👤 Users

* getAllUsers()
* getUserById(p_id)
* getUserByCredentials(p_username, p_password)
* addNewUser(...)
* updateUserById(...)
* deleteUserById(p_id)

---

# 🔥 SPECIAL FUNCTIONS

## ✔ Get Skills by Person

```sql
SELECT * FROM getSkillsByPerson(p_personid)
```

Returns:

* SkillID
* SkillName

---

## ✔ Get Skills by Project

```sql
SELECT * FROM getSkillsByProject(p_projectid)
```

---

# ⚙️ FUNCTION USAGE PATTERNS

## ✔ Get Data

```sql
SELECT * FROM functionName(...)
```

## ✔ Insert

```sql
SELECT functionName(...)
```

## ✔ Update

```sql
SELECT functionName(...)
```

## ✔ Delete

```sql
SELECT functionName(...)
```

---

# 🧠 DATA TYPES MAPPING

| PostgreSQL | C#       |
| ---------- | -------- |
| bigint     | long     |
| varchar    | string   |
| text       | string   |
| boolean    | bool     |
| date       | DateTime |

---

# 🚀 FINAL NOTES

* All database logic is centralized in functions
* No raw SQL should be written in application layer
* All DAL must call functions using SELECT
* Parameter names must match exactly (p_id, p_name, etc.)
* Case sensitivity matters in PostgreSQL

---

# ✅ PURPOSE OF THIS FILE

This document is used to:

✔ Guide AI agents
✔ Prevent schema mismatch
✔ Ensure consistent DAL generation
✔ Avoid runtime errors
✔ Maintain clean architecture
