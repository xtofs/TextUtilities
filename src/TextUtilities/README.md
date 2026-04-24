# TextUtilities

`TextUtilities` is a .NET library for small, focused text helper functions.

## Included utilities

- `TextUtility.NullIfWhiteSpace`
- `TextUtility.NormalizeLineEndings`

## Example

```csharp
using TextUtilities;

var normalized = TextUtility.NormalizeLineEndings("one\r\ntwo\rthree");
var meaningful = TextUtility.NullIfWhiteSpace(" value ");
```