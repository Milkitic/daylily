namespace daylily.Utils;

public static class StringUtil
{
    public static string ToLowerSnake(this string sourceString)
    {
        int additionalLength = 0;

        var span = sourceString.AsSpan();
        for (var i = 0; i < span.Length; i++)
        {
            var c = span[i];
            if (c >= 65 && c <= 90 && i != 0)
            {
                additionalLength++;
            }
        }

        if (additionalLength == 0)
        {
            return sourceString;
        }

        int j = 0;
        return string.Create(sourceString.Length + additionalLength, sourceString, (target, source) =>
        {
            var span1 = source.AsSpan();
            for (var i = 0; i < span1.Length; i++)
            {
                var c = span1[i];
                if (c >= 65 && c <= 90)
                {
                    if (i == 0)
                    {
                        target[0] = (char)(c + 32);
                        continue;
                    }

                    target[i + j] = '_';
                    j++;
                    target[i + j] = (char)(c + 32);
                    continue;
                }

                target[i + j] = c;
            }
        });
    }
}