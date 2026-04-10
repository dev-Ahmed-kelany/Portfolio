using System.Text.RegularExpressions;
using Portfolio.DataAccess;

namespace Portfolio.Business
{
    public static class clsValidation
    {
        // Email validation
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return clsUtility.MatchRegex(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
        }

        // URL validation
        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) 
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        // Phone validation (basic format)
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return clsUtility.MatchRegex(phone, @"^[\d\s\-\+\(\)]+$") && phone.Length >= 7;
        }

        // Check if string is null or empty
        public static bool IsNullOrEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        // Check if string is within length limits
        public static bool IsWithinLength(string value, int min, int max)
        {
            if (value == null)
                return min == 0;

            int length = value.Length;
            return length >= min && length <= max;
        }

        // Validate ID (must be > 0)
        public static bool IsValidId(long id)
        {
            return id > 0;
        }

        // Validate PersonID (must be > 0)
        public static bool IsValidPersonId(long personId)
        {
            return personId > 0;
        }

        // Validate date (should not be default)
        public static bool IsValidDate(DateTime date)
        {
            return date != DateTime.MinValue && date != default(DateTime);
        }

        // Validate date range (startDate <= endDate)
        public static bool IsValidDateRange(DateTime startDate, DateTime? endDate)
        {
            if (!IsValidDate(startDate))
                return false;

            if (endDate.HasValue && !IsValidDate(endDate.Value))
                return false;

            if (endDate.HasValue && startDate > endDate.Value)
                return false;

            return true;
        }

        // Validate UserDTO
        public static bool IsValidUserDTO(UserDTO user)
        {
            if (user == null)
                return false;

            if (IsNullOrEmpty(user.Username) || !IsWithinLength(user.Username, 3, 50))
                return false;

            if (IsNullOrEmpty(user.Password) || !IsWithinLength(user.Password, 5, 100))
                return false;

            if (!IsValidPersonId(user.PersonID))
                return false;

            return true;
        }

        public static bool IsValidUserWithoutPasswordDTO(UserWithoutPasswordDTO user)
        {
            if (user == null)
                return false;

            if (IsNullOrEmpty(user.Username) || !IsWithinLength(user.Username, 3, 50))
                return false;

            if (!IsValidPersonId(user.PersonID))
                return false;

            return true;
        }

        // Validate PersonDTO
        public static bool IsValidPersonDTO(PersonDTO person)
        {
            if (person == null)
                return false;

            if (IsNullOrEmpty(person.Name) || !IsWithinLength(person.Name, 2, 100))
                return false;

            if (!string.IsNullOrEmpty(person.Description) && !IsWithinLength(person.Description, 0, 500))
                return false;

            if (!string.IsNullOrEmpty(person.ImageUrl) && !IsValidUrl(person.ImageUrl))
                return false;

            return true;
        }

        // Validate ContactInfoDTO
        public static bool IsValidContactInfoDTO(ContactInfoDTO contactInfo)
        {
            if (contactInfo == null)
                return false;

            if (!IsValidEmail(contactInfo.Email))
                return false;

            if (IsNullOrEmpty(contactInfo.Phone) || !IsValidPhone(contactInfo.Phone))
                return false;

            if (IsNullOrEmpty(contactInfo.Location) || !IsWithinLength(contactInfo.Location, 2, 100))
                return false;

            if (!IsValidPersonId(contactInfo.PersonID))
                return false;

            return true;
        }

        // Validate AboutDTO
        public static bool IsValidAboutDTO(AboutDTO about)
        {
            if (about == null)
                return false;

            if (IsNullOrEmpty(about.Summary) || !IsWithinLength(about.Summary, 10, 500))
                return false;

            if (IsNullOrEmpty(about.FocusArea) || !IsWithinLength(about.FocusArea, 5, 200))
                return false;

            if (IsNullOrEmpty(about.Highlighted) || !IsWithinLength(about.Highlighted, 5, 200))
                return false;

            if (!IsValidPersonId(about.PersonID))
                return false;

            return true;
        }

        // Validate SocialLinkDTO
        public static bool IsValidSocialLinkDTO(SocialLinkDTO socialLink)
        {
            if (socialLink == null)
                return false;

            if (IsNullOrEmpty(socialLink.Platform) || !IsWithinLength(socialLink.Platform, 2, 50))
                return false;

            if (IsNullOrEmpty(socialLink.Url) || !IsValidUrl(socialLink.Url))
                return false;

            if (IsNullOrEmpty(socialLink.Icon) || !IsWithinLength(socialLink.Icon, 1, 100))
                return false;

            if (!IsValidPersonId(socialLink.PersonID))
                return false;

            return true;
        }

        // Validate SkillDTO
        public static bool IsValidSkillDTO(SkillDTO skill)
        {
            if (skill == null)
                return false;

            if (IsNullOrEmpty(skill.Name) || !IsWithinLength(skill.Name, 2, 100))
                return false;

            if (IsNullOrEmpty(skill.Icon) || !IsWithinLength(skill.Icon, 1, 100))
                return false;

            if (!IsValidId(skill.SkillCategoryID))
                return false;

            return true;
        }

        // Validate SkillCategoryDTO
        public static bool IsValidSkillCategoryDTO(SkillCategoryDTO skillCategory)
        {
            if (skillCategory == null)
                return false;

            if (IsNullOrEmpty(skillCategory.Name) || !IsWithinLength(skillCategory.Name, 2, 100))
                return false;

            return true;
        }

        // Validate JobTitleDTO
        public static bool IsValidJobTitleDTO(JobTitleDTO jobTitle)
        {
            if (jobTitle == null)
                return false;

            if (IsNullOrEmpty(jobTitle.Title) || !IsWithinLength(jobTitle.Title, 2, 100))
                return false;

            if (!IsValidPersonId(jobTitle.PersonID))
                return false;

            return true;
        }

        // Validate ExperienceDTO
        public static bool IsValidExperienceDTO(ExperienceDTO experience)
        {
            if (experience == null)
                return false;

            if (IsNullOrEmpty(experience.Name) || !IsWithinLength(experience.Name, 2, 100))
                return false;

            if (IsNullOrEmpty(experience.Description) || !IsWithinLength(experience.Description, 10, 500))
                return false;

            if (IsNullOrEmpty(experience.CompanyName) || !IsWithinLength(experience.CompanyName, 2, 100))
                return false;

            if (!IsValidDateRange(experience.StartDate, experience.EndDate))
                return false;

            if (!IsValidPersonId(experience.PersonID))
                return false;

            return true;
        }

        // Validate ProjectDTO
        public static bool IsValidProjectDTO(ProjectDTO project)
        {
            if (project == null)
                return false;

            if (IsNullOrEmpty(project.Title) || !IsWithinLength(project.Title, 2, 200))
                return false;

            if (IsNullOrEmpty(project.Description) || !IsWithinLength(project.Description, 10, 500))
                return false;

            if (IsNullOrEmpty(project.Details) || !IsWithinLength(project.Details, 10, 2000))
                return false;

            if (IsNullOrEmpty(project.ImageUrl) || !IsValidUrl(project.ImageUrl))
                return false;

            if (IsNullOrEmpty(project.GitHubUrl) || !IsValidUrl(project.GitHubUrl))
                return false;

            if (IsNullOrEmpty(project.LiveUrl) || !IsValidUrl(project.LiveUrl))
                return false;

            if (!IsValidPersonId(project.PersonID))
                return false;

            return true;
        }

        // Validate PersonSkillDTO
        public static bool IsValidPersonSkillDTO(PersonSkillDTO personSkill)
        {
            if (personSkill == null)
                return false;

            if (!IsValidPersonId(personSkill.PersonID))
                return false;

            if (!IsValidId(personSkill.SkillID))
                return false;

            return true;
        }

        // Validate ProjectSkillDTO
        public static bool IsValidProjectSkillDTO(ProjectSkillDTO projectSkill)
        {
            if (projectSkill == null)
                return false;

            if (!IsValidId(projectSkill.ProjectID))
                return false;

            if (!IsValidId(projectSkill.SkillID))
                return false;

            return true;
        }
    }
}
