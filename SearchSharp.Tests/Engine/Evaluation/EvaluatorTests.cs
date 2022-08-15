using System.Linq.Expressions;
using SearchSharp.Exceptions;
using SearchSharp.Engine.Evaluation;
using SearchSharp.Engine.Parser.Components;
using SearchSharp.Engine.Rules;
using Moq;
using SearchSharp.Tests.Support;

namespace SearchSharp.Tests.Engine.Evaluation;

public class EvaluatorTests {
    public class DummyData : QueryData {
        public enum Sorter {
            First = 0,
            Last = 1
        }

        public string Info { get; set; } = string.Empty;
        public int Integer { get; set; } = 0;
        public float Float { get; set; } = 0.0f;
        public bool Is { get; set; } = false;
        public Sorter Sort { get; set; } = Sorter.First;
    }
    /*
    #region String
    [Fact]
    public void Evaluate_String_Replacement(){
        #region Assemble
        Expression<Func<DummyData, bool>> expected = (d) => d.Info == "replacement";
        var evaluator = new Evaluator<DummyData>(new Dictionary<string, IRule<DummyData>>(), (d, str) => d.Info == str, d => true);
        #endregion

        #region Act
        var evaluated = evaluator.Evaluate("replacement");
        #endregion

        #region Assert
        //same expression
        Assert.Equal(expected.ToString(), evaluated.ToString());

        //expected results
        Assert.True(evaluated.Compile()(new DummyData { Info = "replacement" }));
        Assert.False(evaluated.Compile()(new DummyData { Info = "not" }));
        #endregion
    }

    [Fact]
    public void Evaluate_String_MethodCall(){
        #region Assemble
        Expression<Func<DummyData, bool>> expected = (d) => d.Info.Length == "replacement".Length;
        var evaluator = new Evaluator<DummyData>(new Dictionary<string, IRule<DummyData>>(), (d, str) => d.Info.Length == str.Length, d => true);
        #endregion

        #region Act
        var evaluated = evaluator.Evaluate("replacement");
        #endregion

        #region Assert
        //same expression
        Assert.Equal(expected.ToString(), evaluated.ToString());

        //expected results
        Assert.True(evaluated.Compile()(new DummyData { Info = "replacement" }));
        Assert.False(evaluated.Compile()(new DummyData { Info = "not" }));
        #endregion
    }
    #endregion

    #region Comparison
    [Fact]
    public void Evaluate_Comparison_UnkownRule(){
        #region Assemble
        Expression<Func<DummyData, bool>> defaultExpression = (d) => true;
        var evaluator = new Evaluator<DummyData>(new Dictionary<string, IRule<DummyData>>(), (d, str) => true, defaultExpression);
        #endregion

        #region Act
        var evaluated = evaluator.Evaluate(new ComparisonDirective(DirectiveComparisonOperator.Rule, "unknown", NumericLiteral.Int(10)));
        #endregion

        #region Assert
        Assert.Equal(defaultExpression.ToString(), evaluated.ToString());
        #endregion
    }
    [Theory]
    [ClassData(typeof(EnumClassData<DirectiveComparisonOperator>))]
    public void Evaluate_Comparison_String_UnkownOperatorType(DirectiveComparisonOperator operatorType){
        #region Assemble
        Expression<Func<DummyData, bool>> defaultExpression = (d) => true;
        Expression<Func<DummyData, StringLiteral, bool>>? outExpr = (d,s) => true;
        var mockRule = Mock.Of<IRule<DummyData>>(rule => rule.ComparisonStrRules.TryGetValue(operatorType, out outExpr)  == false);

        var evaluator = new Evaluator<DummyData>(new Dictionary<string, IRule<DummyData>>{
            { "unknown", mockRule }
        }, (d, str) => true, defaultExpression);
        #endregion

        #region Act
        var evaluated = evaluator.Evaluate(new ComparisonDirective(operatorType, "unknown", "something".AsLiteral()));
        #endregion

        #region Assert
        Assert.Equal(defaultExpression.ToString(), evaluated.ToString());
        #endregion
    }
    [Theory]
    [ClassData(typeof(EnumClassData<DirectiveComparisonOperator>))]
    public void Evaluate_Comparison_Numeric_UnkownOperatorType(DirectiveComparisonOperator operatorType){
        #region Assemble
        Expression<Func<DummyData, bool>> defaultExpression = (d) => true;
        Expression<Func<DummyData, NumericLiteral, bool>>? outExpr = (d,s) => true;
        var mockRule = Mock.Of<IRule<DummyData>>(rule => rule.ComparisonNumRules.TryGetValue(operatorType, out outExpr)  == false);

        var evaluator = new Evaluator<DummyData>(new Dictionary<string, IRule<DummyData>>{
            { "unknown", mockRule }
        }, (d, str) => true, defaultExpression);
        #endregion

        #region Act
        var evaluated = evaluator.Evaluate(new ComparisonDirective(operatorType, "unknown", 10.AsLiteral()));
        #endregion

        #region Assert
        Assert.Equal(defaultExpression.ToString(), evaluated.ToString());
        #endregion
    }
    [Theory]
    [ClassData(typeof(EnumClassData<DirectiveComparisonOperator>))]
    public void Evaluate_Comparison_Boolean_UnkownOperatorType(DirectiveComparisonOperator operatorType){
        #region Assemble
        Expression<Func<DummyData, bool>> defaultExpression = (d) => true;
        Expression<Func<DummyData, BooleanLiteral, bool>>? outExpr = (d,s) => true;
        var mockRule = Mock.Of<IRule<DummyData>>(rule => rule.ComparisonBoolRules.TryGetValue(operatorType, out outExpr)  == false);

        var evaluator = new Evaluator<DummyData>(new Dictionary<string, IRule<DummyData>>{
            { "unknown", mockRule }
        }, (d, str) => true, defaultExpression);
        #endregion

        #region Act
        var evaluated = evaluator.Evaluate(new ComparisonDirective(operatorType, "unknown", false.AsLiteral()));
        #endregion

        #region Assert
        Assert.Equal(defaultExpression.ToString(), evaluated.ToString());
        #endregion
    }
    [Theory]
    [ClassData(typeof(EnumClassData<DirectiveComparisonOperator>))]
    public void Evaluate_Comparison_String_OperatorType(DirectiveComparisonOperator operatorType){
        #region Assemble
        Expression<Func<DummyData, bool>> defaultExpression = (d) => true;
        Expression<Func<DummyData, StringLiteral, bool>>? outExpr = (d,str) => false;
        Expression<Func<DummyData, bool>> expected = (d) => false;
        var mockRule = Mock.Of<IRule<DummyData>>(rule => rule.ComparisonStrRules.TryGetValue(operatorType, out outExpr)  == true);

        var evaluator = new Evaluator<DummyData>(new Dictionary<string, IRule<DummyData>> {
            { "known", mockRule }
        }, (d, str) => true, defaultExpression);
        #endregion

        #region Act
        var evaluated = evaluator.Evaluate(new ComparisonDirective(operatorType, "known", "sorted".AsLiteral()));
        #endregion

        #region Assert
        Assert.Equal(expected.ToString(), evaluated.ToString());
        #endregion
    }
    [Theory]
    [ClassData(typeof(EnumClassData<DirectiveComparisonOperator>))]
    public void Evaluate_Comparison_Numeric_OperatorType(DirectiveComparisonOperator operatorType){
        #region Assemble
        Expression<Func<DummyData, bool>> defaultExpression = (d) => true;
        Expression<Func<DummyData, NumericLiteral, bool>>? outExpr = (d,str) => false;
        Expression<Func<DummyData, bool>> expected = (d) => false;
        var mockRule = Mock.Of<IRule<DummyData>>(rule => rule.ComparisonNumRules.TryGetValue(operatorType, out outExpr)  == true);

        var evaluator = new Evaluator<DummyData>(new Dictionary<string, IRule<DummyData>> {
            { "known", mockRule }
        }, (d, str) => true, defaultExpression);
        #endregion

        #region Act
        var evaluated = evaluator.Evaluate(new ComparisonDirective(operatorType, "known", 10.AsLiteral()));
        #endregion

        #region Assert
        Assert.Equal(expected.ToString(), evaluated.ToString());
        #endregion
    }
    [Theory]
    [ClassData(typeof(EnumClassData<DirectiveComparisonOperator>))]
    public void Evaluate_Comparison_Boolean_OperatorType(DirectiveComparisonOperator operatorType){
        #region Assemble
        Expression<Func<DummyData, bool>> defaultExpression = (d) => true;
        Expression<Func<DummyData, BooleanLiteral, bool>>? outExpr = (d,str) => false;
        Expression<Func<DummyData, bool>> expected = (d) => false;
        var mockRule = Mock.Of<IRule<DummyData>>(rule => rule.ComparisonBoolRules.TryGetValue(operatorType, out outExpr)  == true);

        var evaluator = new Evaluator<DummyData>(new Dictionary<string, IRule<DummyData>> {
            { "known", mockRule }
        }, (d, str) => true, defaultExpression);
        #endregion

        #region Act
        var evaluated = evaluator.Evaluate(new ComparisonDirective(operatorType, "known", true.AsLiteral()));
        #endregion

        #region Assert
        Assert.Equal(expected.ToString(), evaluated.ToString());
        #endregion
    }
    
    [Theory]
    [ClassData(typeof(EnumClassData<DirectiveComparisonOperator>))]
    public void Evaluate_Comparison_Replacement(DirectiveComparisonOperator operatorType){
        #region Assemble
        Expression<Func<DummyData, bool>> defaultExpression = (d) => true;
        Expression<Func<DummyData, StringLiteral, bool>>? outStrExpr = (d,str) => d.Info == str.Value;
        Expression<Func<DummyData, NumericLiteral, bool>>? outNumExpr = (d,num) => d.Integer == num.AsInt;
        Expression<Func<DummyData, BooleanLiteral, bool>>? outBoolExpr = (d,@bool) => d.Is && @bool.Value;
        Expression<Func<DummyData, bool>> expectedStr = (d) => d.Info == "value";
        Expression<Func<DummyData, bool>> expectedNum = (d) => d.Integer == 10;
        Expression<Func<DummyData, bool>> expectedBool = (d) => d.Is && true;
        var mockRule = Mock.Of<IRule<DummyData>>(rule => rule.ComparisonStrRules.TryGetValue(operatorType, out outStrExpr)  == true
                                                        && rule.ComparisonNumRules.TryGetValue(operatorType, out outNumExpr) == true
                                                        && rule.ComparisonBoolRules.TryGetValue(operatorType, out outBoolExpr) == true);

        var evaluator = new Evaluator<DummyData>(new Dictionary<string, IRule<DummyData>> {
            { "known", mockRule }
        }, (d, str) => true, defaultExpression);
        #endregion

        #region Act
        var evaluatedStr = evaluator.Evaluate(new ComparisonDirective(operatorType, "known", "value".AsLiteral()));
        var evaluatedNum = evaluator.Evaluate(new ComparisonDirective(operatorType, "known", 10.AsLiteral()));
        var evaluatedBool = evaluator.Evaluate(new ComparisonDirective(operatorType, "known", true.AsLiteral()));
        #endregion

        #region Assert
        Assert.Equal(expectedStr.ToString(), evaluatedStr.ToString());
        Assert.Equal(expectedNum.ToString(), evaluatedNum.ToString());
        Assert.Equal(expectedBool.ToString(), evaluatedBool.ToString());
        #endregion
    }
    #endregion

    #region Numeric
    [Fact]
    public void Evaluate_Numeric_UnknownRule(){
        #region Assemble
        var evaluator = new Evaluator<DummyData>(new Dictionary<string, IRule<DummyData>>(), (d, str) => true);
        var directive = new NumericDirective(new NumericDirective.Operator(DirectiveNumericOperator.GreaterOrEqual, 10.AsLiteral()), "unknown");
        #endregion

        #region Act
        var thrown = Assert.Throws<UnknownRuleException>(() => evaluator.Evaluate(directive));
        #endregion

        #region Assert
        Assert.Equal("unknown", thrown.Identifier);
        #endregion
    }
    [Theory]
    [ClassData(typeof(EnumClassData<DirectiveNumericOperator>))]
    public void Evaluate_Numeric_UnknownOperator(DirectiveNumericOperator operatorType){
        #region Assemble
        Expression<Func<DummyData, NumericLiteral, bool>>? outExpr = (d,num) => false;
        var mockRule = Mock.Of<IRule<DummyData>>(rule => rule.NumericRules.TryGetValue(operatorType, out outExpr) == false);
        var evaluator = new Evaluator<DummyData>(new Dictionary<string, IRule<DummyData>>{
            { "known", mockRule }
        }, (d, str) => true);
        var directive = new NumericDirective(new NumericDirective.Operator(operatorType, 10.AsLiteral()), "known");
        #endregion

        #region Act
        var thrown = Assert.Throws<UnknownRuleDirectiveException>( () => evaluator.Evaluate(directive));
        #endregion

        #region Assert
        Assert.Equal("known", thrown.Identifier);
        Assert.Equal(directive, thrown.Directive);
        #endregion
    }
    [Theory]
    [ClassData(typeof(EnumClassData<DirectiveNumericOperator>))]
    public void Evaluate_Numeric_Replacement(DirectiveNumericOperator operatorType){
        #region Assemble
        Expression<Func<DummyData, bool>> defaultExpression = (d) => true;
        Expression<Func<DummyData, NumericLiteral, bool>>? outExpr = (d,num) => d.Integer == num.AsInt;
        Expression<Func<DummyData, bool>> expected = (d) => d.Integer == 10;
        var mockRule = Mock.Of<IRule<DummyData>>(rule => rule.NumericRules.TryGetValue(operatorType, out outExpr) == true);
        var evaluator = new Evaluator<DummyData>(new Dictionary<string, IRule<DummyData>>{
            { "known", mockRule }
        }, (d, str) => true);
        #endregion

        #region Act
        var evaluated = evaluator.Evaluate(new NumericDirective(new NumericDirective.Operator(operatorType, 10.AsLiteral()), "known"));
        #endregion

        #region Assert
        Assert.Equal(expected.ToString(), evaluated.ToString());
        #endregion
    }
    #endregion
    */
}