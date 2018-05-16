using Firebase.Analytics;
using Foundation;

namespace Gabber.iOS.Helpers
{
    public static class Logger
    {
        // ONBOARDING => {ONBOARDING_SWIPE, 0-4, POSITION}
        // LOGIN => {ATTEMPT, ERROR, SUCCESS}
        // REGISTER => {ATTEMPT, ERROR, SUCCESS}
        // REGISTER_VERIFY: EMAIL_CLIENT => {CLICKED}, LOGIN_BUTTON => {CLICKED}
        // REGISTER_VERIFYING: EMAIL_VERIFICATION => {ERROR, SUCCESS, LOGIN_CLICKED}
        // PROJECT => {SWIPE_REFRESH, 0-N, PROJECT_COUNT}, {PROJECT_SELECTED, Title, PROJECT}
        // PARTICIPANTS:
        //  NO_PARTICIPANTS_SELECTED => {TOAST}, ONE_PARTICIPANT_MODAL => {DISPLAYED, DISMISSED, CONTINUE},
        //  PARTICIPANT_SELECTED => {NAME, EMAIL, STATE, NUM_SELECTED}
        // ADD_PARTICIPANTS: ADD_PARTICIPANT => {NAME, EMAIL}
        // RECORD: TOPIC_SELECTED => {TEXT, ID, PREVIOUS_TEXT, PREVIOUS_ID} 
        // SESSION: UPLOAD_SESSION => {ATTEMPT, ERROR, SUCCESS}, START_RECORDING => {}, STOP_RECORDING => {}
        public static void LOG_EVENT_WITH_ACTION(string eventName, string actionValue, string actionKey="ACTION")
        {
            NSString[] keys = { new NSString(actionKey), new NSString("TIMESTAMP") };
            NSObject[] values = { new NSString(actionValue), new NSString(System.DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()) };
            var parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values, keys, keys.Length);
            Analytics.LogEvent(eventName, parameters);
        }

        public static void LOG_EVENT_WITH_DICT(string eventName, NSDictionary<NSString, NSObject> parameters)
        {
            Analytics.LogEvent(eventName, parameters);
        }
    }
}