namespace Odachi.Build.OptionsFileGenerator;

public static class OptionsFileConstants
{
	public const string NamespaceName = "Odachi.Annotations";

	public const string BranchAttributeFullName = $"{NamespaceName}.{BranchAttributeName}";
    public const string BranchAttributeName = "OptionsContainerAttribute";
    public const string BranchAttributeFileNamePropertyName = "FileName";
    public const string BranchAttributeFormatPropertyName = "Format";
    public const string BranchAttributeKeyPropertyName = "Key";

    public const string LeafAttributeFullName = $"{NamespaceName}.{LeafAttributeName}";
    public const string LeafAttributeName = "OptionsPropertyAttribute";
    public const string LeafAttributeCommentPropertyName = "Comment";
    public const string LeafAttributeDefaultValuePropertyName = "DefaultValue";
}
