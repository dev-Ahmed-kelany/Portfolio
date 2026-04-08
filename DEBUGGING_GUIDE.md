# Debugging Checklist for SkillCategories API

## Critical Issues Fixed

### 1. ? Empty Catch Blocks
**Problem:** All catch blocks were empty - exceptions were silently swallowed
```csharp
catch { }  // BAD - Silent failure!
```
**Solution:** ? Added proper exception logging and re-throw
```csharp
catch (Exception ex) {
    Console.WriteLine($"Error: {ex.Message}");
    throw;
}
```

### 2. ?? Controller Logging
**Problem:** No visibility into what's happening
**Solution:** ? Added comprehensive logging at every step
- Request received
- Validation results
- Database operations
- Success/failure status
- Full exception details

---

## Debugging Steps

### Step 1: Check Database Connection
```
Verify clsSettings.ConnectionString is correctly configured
Check appsettings.json for "DefaultConnection"
```

### Step 2: Verify Stored Procedures Exist
Run in PostgreSQL:
```sql
-- Check if stored procedure exists
SELECT routine_name 
FROM information_schema.routines 
WHERE routine_name = 'getAllSkillCategories';

-- List all functions/procedures
SELECT routine_name 
FROM information_schema.routines 
WHERE routine_schema = 'public';
```

### Step 3: Check Stored Procedure Parameters
For each stored procedure, verify the exact parameter names in PostgreSQL:
```sql
-- Example for getAllSkillCategories function
SELECT * FROM information_schema.parameters 
WHERE specific_name = 'getAllSkillCategories';
```

### Step 4: Test Stored Procedures Directly
```sql
-- Test from PostgreSQL directly
SELECT * FROM "getAllSkillCategories"();
SELECT * FROM "getSkillCategoryById"(1);
SELECT * FROM "addNewSkillCategory"('Test Category');
SELECT * FROM "updateSkillCategoryById"(1, 'Updated Name');
SELECT * FROM "deleteSkillCategoryById"(1);
```

### Step 5: Monitor Application Logs
When making API requests, watch the console output for:
- `Getting all skill categories`
- `Error in getAllSkillCategories: ...`
- Connection errors
- Parameter mismatches

---

## Common Issues & Solutions

### Issue: "Function not found"
**Cause:** Stored procedure name doesn't match PostgreSQL function name
**Solution:** Function names in PostgreSQL are case-sensitive
- PostgreSQL might store as: `"getAllSkillCategories"` or `getallskillcategories`
- Check actual name: `SELECT routine_name FROM information_schema.routines`

### Issue: "Parameter count mismatch"
**Cause:** Number or names of parameters don't match
**Solution:** 
- Use exact parameter names from PostgreSQL
- PostgreSQL is case-sensitive for parameter names
- Example: `p_id` vs `P_ID` vs `pid`

### Issue: "Permission denied"
**Cause:** Database user doesn't have execute permission
**Solution:**
```sql
GRANT EXECUTE ON FUNCTION getAllSkillCategories() TO your_user;
```

### Issue: Empty results
**Cause:** Column names in SELECT don't match what C# is looking for
**Solution:**
- The C# code looks for: `reader.GetOrdinal("ID")`, `reader.GetOrdinal("Name")`
- PostgreSQL function must return columns named exactly: `ID`, `Name`
- Check with: `SELECT * FROM "getAllSkillCategories"() LIMIT 1;`

---

## API Testing Guide

### Test 1: Get All (GET)
```
URL: http://localhost:5000/api/skillcategories
Method: GET
Expected: 200 OK + list of categories (or empty list)
```

### Test 2: Get By ID (GET)
```
URL: http://localhost:5000/api/skillcategories/1
Method: GET
Expected: 200 OK + category data OR 404 Not Found
```

### Test 3: Create (POST)
```
URL: http://localhost:5000/api/skillcategories
Method: POST
Body: { "name": "Web Development" }
Expected: 201 Created + new ID
```

### Test 4: Update (PUT)
```
URL: http://localhost:5000/api/skillcategories/1
Method: PUT
Body: { "name": "Updated Name" }
Expected: 200 OK + success message OR 404 Not Found
```

### Test 5: Delete (DELETE)
```
URL: http://localhost:5000/api/skillcategories/1
Method: DELETE
Expected: 200 OK + success message OR 404 Not Found
```

---

## What To Check First

1. ? **appsettings.json** - Is connection string correct?
2. ? **Database** - Are stored procedures created?
3. ? **Procedure Names** - Exactly match function names in PostgreSQL
4. ? **Parameter Names** - Match `p_id`, `p_name` exactly
5. ? **Return Columns** - Must be `ID` and `Name` for SkillCategories
6. ? **Logs** - Check console for actual error messages
7. ? **Permissions** - User can execute the procedures

---

## If Still Not Working

1. Run the stored procedures directly in PostgreSQL
2. Check the console logs in Visual Studio output window
3. The error message will tell you exactly what's wrong
4. Common issues: wrong function name case, missing columns, parameter mismatch
