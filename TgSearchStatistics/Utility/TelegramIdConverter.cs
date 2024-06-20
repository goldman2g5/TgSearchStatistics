namespace TgSearchStatistics.Utility
{
    public static class TelegramIdConverter
    {
        public static long ToPyrogram(long wTelegramChannelId)
        {
            // Pyrogram format requires the ID to be negative with the "-100" prefix
            if (wTelegramChannelId > 0)
            {
                // Construct the Pyrogram ID by adding the "-100" prefix numerically
                string prefixedIdStr = "-100" + wTelegramChannelId.ToString();
                if (long.TryParse(prefixedIdStr, out long pyrogramId))
                {
                    return pyrogramId;
                }
                else
                {
                    throw new ArgumentException("Failed to convert to Pyrogram format.", nameof(wTelegramChannelId));
                }
            }
            else
            {
                // It's already in Pyrogram format or doesn't require conversion
                return wTelegramChannelId;
            }
        }

        public static long ToWTelegramClient(long pyrogramChannelId)
        {
            string channelIdStr = pyrogramChannelId.ToString();
            // Check if the ID is in Pyrogram format (negative and starts with "-100")
            if (channelIdStr.StartsWith("-100"))
            {
                // Remove the "-100" prefix and convert back to long
                if (long.TryParse(channelIdStr.Substring(4), out long numericId))
                {
                    return numericId;
                }
                else
                {
                    throw new ArgumentException("Invalid Pyrogram channel ID format.", nameof(pyrogramChannelId));
                }
            }
            else
            {
                // It's already in WTelegramClient format or doesn't require conversion
                return pyrogramChannelId;
            }
        }
    }
}
