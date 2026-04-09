using System;
using Portfolio.DataAccess;

namespace Portfolio.Business
{
    public class clsPerson
    {
        private readonly IPerson __Person;

        public clsPerson(IPerson Person)
        {
            __Person = Person;
        }

        public async Task<List<PersonDTO>> getAllPeople()
        {
            var People = await __Person.getAll();
            return People;
        }

        public async Task<PersonDTO> getPersonById(long ID)
        {
            var Person = await __Person.getById(ID);
            return Person;
        }

        public async Task<long> addNewPerson(PersonDTO person)
        {
            if (!clsValidation.IsValidPersonDTO(person))
                return 0;

            var newId = await __Person.addNew(person);
            return newId;
        }

        public async Task<bool> updatePersonById(PersonDTO person)
        {
            if (!clsValidation.IsValidPersonDTO(person))
                return false;

            if (!clsValidation.IsValidId(person.ID))
                return false;

            var result = await __Person.updateById(person);
            return result;
        }

        public async Task<bool> deletePersonById(long ID)
        {
            if (!clsValidation.IsValidId(ID))
                return false;

            var result = await __Person.deleteById(ID);
            return result;
        }
    }
}
