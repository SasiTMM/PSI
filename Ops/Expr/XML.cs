using System.Xml.Linq;

namespace PSI;

public class ExprXML : Visitor<XElement> {
   public ExprXML (string expression)
      => mExpression = expression;

   public override XElement Visit (NLiteral literal) {
      XElement node = new XElement ("Literal");
      node.SetAttributeValue ("Value", literal.Value.Text);
      node.SetAttributeValue("Type", literal.Type);
      return node;
   }

   public override XElement Visit (NIdentifier identifier) {
      XElement node = new XElement ("Ident");
      node.SetAttributeValue ("Name", identifier.Name);
      node.SetAttributeValue ("Type", identifier.Type);
      return node;
   }

   public override XElement Visit (NUnary unary) {
      XElement node = new XElement ("Unary",
                       new XElement (unary.Expr.Accept (this)));
      node.SetAttributeValue ("Op", unary.Op);
      return node;
   }

   public override XElement Visit (NBinary binary) {
      XElement node = new XElement ("Binary",
                        new XElement (binary.Left.Accept (this)),
                        new XElement (binary.Right.Accept (this)));
      node.SetAttributeValue ("Op", binary.Op.Kind);
      node.SetAttributeValue ("Type", binary.Type);
      return node;
   }

   public void SaveTo (string file, XElement node)
      => File.WriteAllText (file, $"{node.ToString ()}");

   readonly string mExpression;
}
