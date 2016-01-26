using System.Diagnostics.CodeAnalysis;
using Dematt.Airy.Tests.Domain;
using NUnit.Framework;

namespace Dematt.Airy.Tests
{
    /// <summary>
    /// Tests that the base domain class inherited by all domain classes implements the overridden Equals method correctly.
    /// </summary>    
    [TestFixture]
    public class EntityEqualityTests
    {
        /// <summary>
        /// Two null domain objects should be considered equal.
        /// </summary>
        [Test]
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse", Justification = "This is what we are testing as we have overridden equals.")]
        public void EqualsWithTwoNullObjectsReturnsTrue()
        {
            const SimpleDomainObject obj1 = null;
            const SimpleDomainObject obj2 = null;

            bool equality = Equals(obj1, obj2);
            Assert.AreEqual(true, equality);
        }

        /// <summary>
        /// Two domain object where one is null and the other initialised should be considered not equal.
        /// </summary>
        [Test]
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse", Justification = "This is what we are testing as we have overridden equals.")]
        public void EqualsWithOneNullObjectReturnFalse()
        {
            const SimpleDomainObject obj1 = null;
            var obj2 = new SimpleDomainObject();

            var equality = Equals(obj1, obj2);
            Assert.AreEqual(false, equality);
        }

        /// <summary>
        /// Two different transient domain object should be considered not equal.
        /// </summary>
        [Test]
        public void EqualsWithTwoTransientObjectsReturnsFalse()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new SimpleDomainObject();

            var equality = Equals(obj1, obj2);
            Assert.AreEqual(false, equality);
        }

        /// <summary>
        /// Two transient domain object that point to the same reference should be considered equal.
        /// </summary>
        [Test]
        public void EqualsWithTwoTransientObjectsThatAreTheSameReferenceReturnsTrue()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = obj1;

            var equality = Equals(obj1, obj2);
            Assert.AreEqual(true, equality);
        }

        /// <summary>
        /// Two domain object where one is transient and the other persisted should be considered not equal.
        /// </summary>
        [Test]
        public void EqualsWithOneTransientObjectReturnsFalse()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new SimpleDomainObject();
            obj2.SetId(1);

            var equality = Equals(obj1, obj2);
            Assert.AreEqual(false, equality);
        }

        /// <summary>
        /// Two domain object that are persisted and have the same Id's should be considered equal.
        /// </summary>
        [Test]
        public void EqualsWithSameIdsReturnsTrue()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new SimpleDomainObject();
            obj1.SetId(1);
            obj2.SetId(1);

            var equality = Equals(obj1, obj2);
            Assert.AreEqual(true, equality);
        }

        /// <summary>
        /// Two domain object that are persisted and have different Id's should be considered not equal.
        /// </summary>
        [Test]
        public void EqualsWithDifferentIdsReturnsFalse()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new SimpleDomainObject();
            obj1.SetId(1);
            obj2.SetId(2);

            var equality = Equals(obj1, obj2);
            Assert.AreEqual(false, equality);
        }

        /// <summary>
        /// Two domain object that are persisted, have the same Id's and where one is a sub-class of the other should be considered equal.
        /// This is so that NHibernate proxies of objects that have the same Id as the class they are 'proxying' are considered to be equal.
        /// </summary>
        [Test]
        public void EqualsWithSameIdsInSubclassReturnsTrue()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new SubSimpleDomainObject();

            obj1.SetId(1);
            obj2.SetId(1);

            bool equality = Equals(obj1, obj2);
            Assert.AreEqual(true, equality);
        }

        /// <summary>
        /// Two domain object that are persisted, have different Id's and where one is a sub-class of the other should be considered not equal.
        /// </summary>
        [Test]
        public void EqualsWithDifferentIdsInSubclassReturnsFalse()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new SubSimpleDomainObject();

            obj1.SetId(1);
            obj2.SetId(2);

            bool equality = Equals(obj1, obj2);
            Assert.AreEqual(false, equality);
        }

        /// <summary>
        /// Two domain objects that are persisted, have the same Id's but are disparate classes should be considered not equal.
        /// </summary>        
        [Test]
        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global", Justification = "Thanks R#, but that's what we are testing for.")]
        public void EqualsWithSameIdsInDisparateClassesReturnsFalse()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new OtherDomainObject();
            obj1.SetId(1);
            obj2.SetId(1);

            bool equality = Equals(obj1, obj2);
            Assert.AreEqual(false, equality);
        }

        /// <summary>
        /// Two domain objects that are persisted, have different Id's and are disparate classes should be considered not equal.
        /// </summary>
        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global", Justification = "Thanks R#, but that's what we are testing for.")]
        [Test]
        public void EqualsWithDifferentIdsInDisparateClassesReturnsFalse()
        {
            var obj1 = new SimpleDomainObject();
            var obj2 = new OtherDomainObject();
            obj1.SetId(1);
            obj2.SetId(2);

            bool equality = Equals(obj1, obj2);
            Assert.AreEqual(false, equality);
        }
    }
}
