# Google Cloud GEmail Dispatcher

## Sends emails using the Gmail API and Google Cloud Functions.

- A .NET Core 3.1 equivalent of the OAuth authorization process from https://github.com/GoogleCloudPlatform/cloud-functions-gmail-nodejs
- Check https://codelabs.developers.google.com/codelabs/intelligent-gmail-processing#3 for instructions on how to create an OAuth client to authorize access to Gmail

_The Cloud Functions use HTTP trigger and unauthenticated invocations_

- To test the cloud functions
    1. From the Google Cloud Functions dashboard, click on the OAuthInit function
    2. Go to TRIGGER tab and click on the Trigger URL
    3. After the 2nd step, you should be redirected to an accounts.google.com page, choose a Google Account
    4. If you are redirected to a "Google didn't verify the application" page, click on Advanced and then on Access [gcloud-function-region]-[gcloud-function-  projectid].cloudfunctions.net
    5. Review the scopes the cloud function wants access to, then click on Continue if everything is in order
    6. You should be redirected to a page with the successful message: "Saved the access and refresh tokens in Google Cloud Datastore!"
    7. Go back to Google Cloud Functions dashboard, click on the EmailDispatcher function
    8. Go to TESTING tab and use a request like in the example, replacing [string] with your data (if you don't want to input any value into CC or BCC you must use an empty string value: ""):

    ```
    {
        "From":"[required - your gmail address]",
        "To":"[required - an email address]",
        "ReplyTo":"[required - an email address or the same address from To]",
        "CC":"[optional - an email address]",
        "BCC":"[optional -  an email address]",
        "Subject":"[required - email's subject]",
        "Content":"[required - email's content]"
    }
    ```

    9. Click TEST THE FUNCTION button

_If you receive a 500 HTTP STATUS CODE due to an Environment Variable that is null, edit the Google Cloud Function and check if there's a space after any Environment Variable_
