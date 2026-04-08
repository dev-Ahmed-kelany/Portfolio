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
            var newId = await __Person.addNew(person);
            return newId;
        }

        public async Task<bool> updatePersonById(PersonDTO person)
        {
            var result = await __Person.updateById(person);
            return result;
        }

        public async Task<bool> deletePersonById(long ID)
        {
            var result = await __Person.deleteById(ID);
            return result;
        }
    }
}
