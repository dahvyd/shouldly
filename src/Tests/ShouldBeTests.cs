﻿using System;
using NUnit.Framework;
using Shouldly;

namespace Tests
{
	[TestFixture]
	public class ShouldBeTests
	{
		[Test]
		public void ShouldBe_WhenTrue_ShouldNotThrow()
		{
			true.ShouldBe(true);
		}

        [Test]
		public void ShouldBe_WhenFalse_ShouldThrow()
		{
            Should.Error(() =>
            true.ShouldBe(false),
            "true should be False but was True");
		}

        [Test]
        public void ShouldBeSatsfiedBy_WhenTrue_ShouldNotThrow()
        {
            true.ShouldBeSatisfiedBy(x => x);
        }

        [Test]
        public void ShouldBeSatsfiedBy_WhenFalse_ShouldThrow()
        {
            Should.Error(() =>
            true.ShouldBeSatisfiedBy(x => x),
            "true should be satisfied by (x=>x) but was not");
        }

        [Test]
        public void ShouldNotBeSatsfiedBy_WhenTrue_ShouldNotThrow()
        {
            true.ShouldNotBeSatisfiedBy(x => !x);
        }

        [Test]
        public void ShouldNotBeSatsfiedBy_WhenFalse_ShouldThrow()
        {
            Should.Error(() =>
            true.ShouldNotBeSatisfiedBy(x => x),
            "true should not be satisfied by (x=>x) but was");
        }

		[Test]
		public void ShouldNotBe_WhenTrue_ShouldNotThrow()
		{
			"this string".ShouldNotBe("some other string");
		}

		[Test]
		public void Should_WithNumbers_ShouldAllowTolerance()
		{
			Math.PI.ShouldNotBe(3.14);
			Math.PI.ShouldBe(3.14, 0.01);
			((float)Math.PI).ShouldBe(3.14f, 0.01);
		}

		[Test]
		public void ShouldBe_GreaterThan()
		{
			7.ShouldBeGreaterThan(1);
			1.ShouldBeLessThan(7);
			Assert.Throws<AssertionException>(() => 7.ShouldBeLessThan(0));
			Assert.Throws<AssertionException>(() => 0.ShouldBeGreaterThan(7));
		}

		[Test]
		public void ShouldBe_GreaterThanOrEqualTo()
		{
			7.ShouldBeGreaterThanOrEqualTo(1);
			1.ShouldBeGreaterThanOrEqualTo(1);
			Assert.Throws<AssertionException>(() => 0.ShouldBeGreaterThanOrEqualTo(1));
		}

		[Test]
		public void ShouldBeTypeOf_ShouldNotThrowForStrings() 
		{
			"Sup yo".ShouldBeTypeOf(typeof(string));
		}

        [Test]
        public void ShouldBeTypeOfWithGenericParameter_ShouldNotThrowForStrings()
        {
            "Sup yo".ShouldBeTypeOf<string>();
        }

		[Test]
		public void ShouldNotBeTypeOf_ShouldNotThrowForNonMatchingType() 
		{
			"Sup yo".ShouldNotBeTypeOf(typeof(int));
		}

        [Test]
        public void ShouldNotBeTypeOfWithGenericParameter_ShouldNotThrowForNonMatchingTypes()
        {
            "Sup yo".ShouldNotBeTypeOf<int>();
        }

		class MyBase{ }
		class MyThing : MyBase { }

		[Test]
		public void ShouldBeTypeOf_ShouldNotThrowForInheritance() 
		{
			new MyThing().ShouldBeTypeOf<MyBase>();
		}
	}
}
