using DotJEM.NUnit3.Legacy;
using NUnit.Framework;
using System.Drawing;
using DotJEM.NUnit3.Constraints.Objects;

namespace DotJEM.NUnit3.Tests.Legacy.Constraints
{
    public record Line(Point From, Point To)
    {
        public Point To { get; } = To;
        public Point From { get; } = From;
    }

    public record Polygon(string Name, Point[] Points)
    {
        public Point[] Points { get; } = Points;
        public string Name { get; } = Name;
    }

    public class PropertiesEqualsConstraintTest
    {
        [Test]
        public void Test1()
        {
            Polygon square1 = new Polygon("Square", new Point[] { new Point(1, 1), new Point(1, 2), new Point(2, 2), new Point(2, 1) });
            Polygon square2 = new Polygon("Square", new Point[] { new Point(1, 1), new Point(1, 2), new Point(2, 2), new Point(2, 1) });
            Assert.That(square1, ObjectHas.Properties.EqualTo(square2));
        }

        [Test, Explicit("Designed to fail")]
        public void Test0()
        {
            Assert.That(new Point(1,1), ObjectHas.Properties.EqualTo(new Point(2, 2)));
        }

        [Test, Explicit("Designed to fail")]
        public void Test2()
        {
            Polygon square1 = new Polygon("Square", new Point[] { new Point(1, 1), new Point(1, 2), new Point(2, 2), new Point(2, 1) });
            Polygon square2 = new Polygon("Square", new Point[] { new Point(1, 1), new Point(1, 2), new Point(2, 20), new Point(2, 1) });
            Assert.That(square1, ObjectHas.Properties.EqualTo(square2));
        }

        [Test, Explicit("Designed to fail")]
        public void Test3()
        {
            Polygon square1 = new Polygon("Square", new Point[] { new Point(1, 1), new Point(1, 2), new Point(2, 2), new Point(2, 1) });
            Polygon square2 = new Polygon("Square", new Point[] { new Point(1, 1), new Point(1, 22), new Point(22, 22), new Point(22, 1) });
            Assert.That(square1, ObjectHas.Properties.EqualTo(square2));
        }
    }
}