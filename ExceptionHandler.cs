// Type: FlickrNet.ExceptionHandler
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using FlickrNet.Exceptions;
using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public static class ExceptionHandler
  {
    public static Exception CreateResponseException(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      reader.MoveToElement();
      if (!reader.ReadToDescendant("err"))
        throw new XmlException("No error element found in XML");
      int code = 0;
      string message = (string) null;
      while (reader.MoveToNextAttribute())
      {
        if (reader.LocalName == "code")
        {
          try
          {
            code = int.Parse(reader.Value, NumberStyles.Any, (IFormatProvider) NumberFormatInfo.InvariantInfo);
          }
          catch (FormatException ex)
          {
            throw new FlickrException("Invalid value found in code attribute. Value '" + (object) code + "' is not an integer");
          }
        }
        else if (reader.LocalName == "msg")
          message = reader.Value;
      }
      return (Exception) ExceptionHandler.CreateException(code, message);
    }

    private static FlickrApiException CreateException(int code, string message)
    {
      switch (code)
      {
        case 96:
          return (FlickrApiException) new InvalidSignatureException(message);
        case 97:
          return (FlickrApiException) new MissingSignatureException(message);
        case 98:
          return (FlickrApiException) new LoginFailedInvalidTokenException(message);
        case 99:
          return (FlickrApiException) new UserNotLoggedInInsufficientPermissionsException(message);
        case 100:
          return (FlickrApiException) new InvalidApiKeyException(message);
        case 105:
          return (FlickrApiException) new ServiceUnavailableException(message);
        case 111:
          return (FlickrApiException) new FormatNotFoundException(message);
        case 112:
          return (FlickrApiException) new MethodNotFoundException(message);
        case 114:
        case 115:
          return new FlickrApiException(code, message);
        case 116:
          return (FlickrApiException) new BadUrlFoundException(message);
        default:
          return ExceptionHandler.CreateExceptionFromMessage(code, message);
      }
    }

    private static FlickrApiException CreateExceptionFromMessage(int code, string message)
    {
      switch (message)
      {
        case "Photo not found":
        case "Photo not found.":
          return (FlickrApiException) new PhotoNotFoundException(code, message);
        case "Photoset not found":
        case "Photoset not found.":
          return (FlickrApiException) new PhotosetNotFoundException(code, message);
        case "Permission Denied":
          return (FlickrApiException) new PermissionDeniedException(code, message);
        case "User not found":
        case "User not found.":
          return (FlickrApiException) new UserNotFoundException(code, message);
        default:
          return new FlickrApiException(code, message);
      }
    }
  }
}
