using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ZoomNet.Models;
using ZoomNet.Utilities;

namespace ZoomNet.Resources
{
	/// <summary>
	/// Allows you to manage cloud recordings.
	/// </summary>
	/// <remarks>
	/// See <a href="https://marketplace.zoom.us/docs/api-reference/zoom-api/cloud-recording/">Zoom documentation</a> for more information.
	/// </remarks>
	public class CloudRecordings : ICloudRecordings
	{
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="CloudRecordings" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal CloudRecordings(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Retrieve all cloud recordings for a user.
		/// </summary>
		/// <param name="userId">The user Id or email address.</param>
		/// <param name="queryTrash">Indicates if you want to list recordings from trash.</param>
		/// <param name="from">The start date.</param>
		/// <param name="to">The end date.</param>
		/// <param name="recordsPerPage">The number of records returned within a single API call.</param>
		/// <param name="page">The current page number of returned records.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Recording">recordings</see>.
		/// </returns>
		[Obsolete("Zoom is in the process of deprecating the \"page number\" and \"page count\" fields.")]
		public Task<PaginatedResponseWithTokenAndDateRange<Recording>> GetRecordingsForUserAsync(string userId, bool queryTrash = false, DateTime? from = null, DateTime? to = null, int recordsPerPage = 30, int page = 1, CancellationToken cancellationToken = default)
		{
			if (recordsPerPage < 1 || recordsPerPage > 300)
			{
				throw new ArgumentOutOfRangeException(nameof(recordsPerPage), "Records per page must be between 1 and 300");
			}

			return _client
				.GetAsync($"users/{userId}/recordings")
				.WithArgument("trash", queryTrash.ToString().ToLower())
				.WithArgument("from", from?.ToString("yyyy-mm-dd"))
				.WithArgument("to", to?.ToString("yyyy-mm-dd"))
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("page", page)
				.WithCancellationToken(cancellationToken)
				.AsPaginatedResponseWithTokenAndDateRange<Recording>("meetings");
		}

		/// <summary>
		/// Retrieve all cloud recordings for a user.
		/// </summary>
		/// <param name="userId">The user Id or email address.</param>
		/// <param name="queryTrash">Indicates if you want to list recordings from trash.</param>
		/// <param name="from">The start date.</param>
		/// <param name="to">The end date.</param>
		/// <param name="recordsPerPage">The number of records returned within a single API call.</param>
		/// <param name="pagingToken">The paging token.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Recording">recordings</see>.
		/// </returns>
		public Task<PaginatedResponseWithTokenAndDateRange<Recording>> GetRecordingsForUserAsync(string userId, bool queryTrash = false, DateTime? from = null, DateTime? to = null, int recordsPerPage = 30, string pagingToken = null, CancellationToken cancellationToken = default)
		{
			if (recordsPerPage < 1 || recordsPerPage > 300)
			{
				throw new ArgumentOutOfRangeException(nameof(recordsPerPage), "Records per page must be between 1 and 300");
			}

			return _client
				.GetAsync($"users/{userId}/recordings")
				.WithArgument("trash", queryTrash.ToString().ToLower())
				.WithArgument("from", from?.ToString("yyyy-mm-dd"))
				.WithArgument("to", to?.ToString("yyyy-mm-dd"))
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("next_page_token", pagingToken)
				.WithCancellationToken(cancellationToken)
				.AsPaginatedResponseWithTokenAndDateRange<Recording>("meetings");
		}

		/// <summary>
		/// Retrieve all cloud recordings for a meeting or webinar.
		/// </summary>
		/// <param name="meetingId">The meeting Id or UUID.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>Details of recordings made for a particular meeding or webinar.</returns>
		public Task<Recording> GetRecordingsAsync(string meetingId, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"meetings/{meetingId}/recordings")
				.WithCancellationToken(cancellationToken)
				.AsObject<Recording>();
		}

		/// <summary>
		/// Move recordings for a meeting to trash.
		/// </summary>
		/// <param name="meetingId">The meeting Id or UUID.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task MoveRecordingsToTrashAsync(string meetingId, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"meetings/{meetingId}/recordings")
				.WithArgument("action", "trash")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Permanently delete recordings for a meeting.
		/// </summary>
		/// <param name="meetingId">The meeting Id or UUID.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteRecordingsAsync(string meetingId, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"meetings/{meetingId}/recordings")
				.WithArgument("action", "delete")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Move a specific recording file for a meeting to trash.
		/// </summary>
		/// <param name="meetingId">The meeting Id or UUID.</param>
		/// <param name="recordingId">The recording id.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task MoveRecordingToTrashAsync(string meetingId, string recordingId, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"meetings/{meetingId}/recordings/{recordingId}")
				.WithArgument("action", "trash")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Permanently delete a specific recording file for a meeting.
		/// </summary>
		/// <param name="meetingId">The meeting Id or UUID.</param>
		/// <param name="recordingId">The recording id.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteRecordingAsync(string meetingId, string recordingId, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"meetings/{meetingId}/recordings/{recordingId}")
				.WithArgument("action", "delete")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Recover all deleted recordings of a specific meeting from trash.
		/// </summary>
		/// <param name="meetingId">The meeting Id or UUID.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		/// <remarks>Zoom allows recordings to be recovered from trash for up to 30 days from deletion date.</remarks>
		public Task RecoverRecordingsAsync(string meetingId, CancellationToken cancellationToken = default)
		{
			return _client
				.PutAsync($"meetings/{meetingId}/recordings/status")
				.WithArgument("action", "recover")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Recover a specific recording file of a meeting.
		/// </summary>
		/// <param name="meetingId">The meeting Id or UUID.</param>
		/// <param name="recordingId">The recording id.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		/// <remarks>Zoom allows recordings to be recovered from trash for up to 30 days from deletion date.</remarks>
		public Task RecoverRecordingAsync(string meetingId, string recordingId, CancellationToken cancellationToken = default)
		{
			return _client
				.PutAsync($"meetings/{meetingId}/recordings/{recordingId}/status")
				.WithArgument("action", "recover")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Retrieve settings applied to a meeting's cloud recording.
		/// </summary>
		/// <param name="meetingId">The meeting Id or UUID.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="RecordingSettings" />.
		/// </returns>
		public Task<RecordingSettings> GetRecordingSettingsAsync(string meetingId, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"meetings/{meetingId}/recordings/settings")
				.WithCancellationToken(cancellationToken)
				.AsObject<RecordingSettings>();
		}

		/// <summary>
		/// Retrieve all registrants for a recording.
		/// </summary>
		/// <param name="meetingId">The meeting Id or UUID.</param>
		/// <param name="recordsPerPage">The number of records returned within a single API call.</param>
		/// <param name="page">The current page number of returned records.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Registrant">registrants</see>.
		/// </returns>
		[Obsolete("Zoom is in the process of deprecating the \"page number\" and \"page count\" fields.")]
		public Task<PaginatedResponse<Registrant>> GetRecordingRegistrantsAsync(string meetingId, int recordsPerPage = 30, int page = 1, CancellationToken cancellationToken = default)
		{
			if (recordsPerPage < 1 || recordsPerPage > 300)
			{
				throw new ArgumentOutOfRangeException(nameof(recordsPerPage), "Records per page must be between 1 and 300");
			}

			return _client
				.GetAsync($"meetings/{meetingId}/recordings/registrants")
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("page", page)
				.WithCancellationToken(cancellationToken)
				.AsPaginatedResponse<Registrant>("registrants");
		}

		/// <summary>
		/// Retrieve all registrants for a recording.
		/// </summary>
		/// <param name="meetingId">The meeting Id or UUID.</param>
		/// <param name="recordsPerPage">The number of records returned within a single API call.</param>
		/// <param name="pagingToken">The paging token.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Registrant">registrants</see>.
		/// </returns>
		public Task<PaginatedResponseWithToken<Registrant>> GetRecordingRegistrantsAsync(string meetingId, int recordsPerPage = 30, string pagingToken = null, CancellationToken cancellationToken = default)
		{
			if (recordsPerPage < 1 || recordsPerPage > 300)
			{
				throw new ArgumentOutOfRangeException(nameof(recordsPerPage), "Records per page must be between 1 and 300");
			}

			return _client
				.GetAsync($"meetings/{meetingId}/recordings/registrants")
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("next_page_token", pagingToken)
				.WithCancellationToken(cancellationToken)
				.AsPaginatedResponseWithToken<Registrant>("registrants");
		}

		/// <summary>
		/// Add a registrant to an on-demand recording.
		/// </summary>
		/// <param name="meetingId">The meeting ID.</param>
		/// <param name="email">Registrant's email address.</param>
		/// <param name="firstName">Registrant's first name.</param>
		/// <param name="lastName">Registrant's last name.</param>
		/// <param name="address">Registrant's address.</param>
		/// <param name="city">Registrant's city.</param>
		/// <param name="country">Registrant's country.</param>
		/// <param name="zip">Registrant's zip/postal code.</param>
		/// <param name="state">Registrant's state/province.</param>
		/// <param name="phone">Registrant's phone number.</param>
		/// <param name="industry">Registrant's industry.</param>
		/// <param name="organization">Registrant's organization.</param>
		/// <param name="jobTitle">Registrant's job title.</param>
		/// <param name="purchasingTimeFrame">This field can be included to gauge interest of attendees towards buying your product or service.</param>
		/// <param name="roleInPurchaseProcess">Registrant's role in purchase decision.</param>
		/// <param name="numberOfEmployees">Number of employees.</param>
		/// <param name="comments">A field that allows registrants to provide any questions or comments that they might have.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// A <see cref="RecordingRegistration" />.
		/// </returns>
		public Task<RecordingRegistration> AddRegistrantAsync(long meetingId, string email, string firstName, string lastName, string address, string city, string country, string zip, string state, string phone, string industry, string organization, string jobTitle, string purchasingTimeFrame, string roleInPurchaseProcess, string numberOfEmployees, string comments, CancellationToken cancellationToken = default)
		{
			var data = new JObject();
			data.AddPropertyIfValue("email", email);
			data.AddPropertyIfValue("first_name", firstName);
			data.AddPropertyIfValue("last_name", lastName);
			data.AddPropertyIfValue("address", address);
			data.AddPropertyIfValue("city", city);
			data.AddPropertyIfValue("country", country);
			data.AddPropertyIfValue("zip", zip);
			data.AddPropertyIfValue("state", state);
			data.AddPropertyIfValue("phone", phone);
			data.AddPropertyIfValue("industry", industry);
			data.AddPropertyIfValue("org", organization);
			data.AddPropertyIfValue("job_title", jobTitle);
			data.AddPropertyIfValue("purchasing_time_frame", purchasingTimeFrame);
			data.AddPropertyIfValue("role_in_purchasing_process", roleInPurchaseProcess);
			data.AddPropertyIfValue("no_of_employees", numberOfEmployees);
			data.AddPropertyIfValue("comments", comments);

			return _client
				.PostAsync($"meetings/{meetingId}/recordings/registrants")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<RecordingRegistration>();
		}

		/// <summary>
		/// Approve a registration for a meeting.
		/// </summary>
		/// <param name="meetingId">The meeting ID.</param>
		/// <param name="registrantId">The registrant ID.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task ApproveRegistrantAsync(long meetingId, string registrantId, CancellationToken cancellationToken = default)
		{
			return ApproveRegistrantsAsync(meetingId, new[] { registrantId }, cancellationToken);
		}

		/// <summary>
		/// Approve multiple registrations for a meeting.
		/// </summary>
		/// <param name="meetingId">The meeting ID.</param>
		/// <param name="registrantIds">ID for each registrant to be approved.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task ApproveRegistrantsAsync(long meetingId, IEnumerable<string> registrantIds, CancellationToken cancellationToken = default)
		{
			return UpdateRegistrantsStatusAsync(meetingId, registrantIds, "approve", cancellationToken);
		}

		/// <summary>
		/// Reject a registration for a meeting.
		/// </summary>
		/// <param name="meetingId">The meeting ID.</param>
		/// <param name="registrantId">The registrant ID.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task RejectRegistrantAsync(long meetingId, string registrantId, CancellationToken cancellationToken = default)
		{
			return RejectRegistrantsAsync(meetingId, new[] { registrantId }, cancellationToken);
		}

		/// <summary>
		/// Reject multiple registrations for a meeting.
		/// </summary>
		/// <param name="meetingId">The meeting ID.</param>
		/// <param name="registrantIds">ID for each registrant to be rejected.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task RejectRegistrantsAsync(long meetingId, IEnumerable<string> registrantIds, CancellationToken cancellationToken = default)
		{
			return UpdateRegistrantsStatusAsync(meetingId, registrantIds, "deny", cancellationToken);
		}

		/// <summary>
		/// Download the recording file.
		/// </summary>
		/// <param name="recordingFile">The recording file to download.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Stream"/> containing the file.
		/// </returns>
		public async Task<Stream> DownloadFileAsync(RecordingFile recordingFile, CancellationToken cancellationToken = default)
		{
			var tokenHandler = _client.Filters.OfType<ITokenHandler>().SingleOrDefault();

			var requestUri = recordingFile.DownloadUrl;
			if (tokenHandler != null) requestUri += "?access_token=" + tokenHandler.Token;

			var response = await _client.BaseClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
			return await response.Content.ReadAsStreamAsync();
		}

		private Task UpdateRegistrantsStatusAsync(long meetingId, IEnumerable<string> registrantIds, string status, CancellationToken cancellationToken = default)
		{
			var data = new JObject();
			data.AddPropertyIfValue("action", status);
			data.AddPropertyIfValue("registrants", registrantIds.ToArray());

			return _client
				.PutAsync($"meetings/{meetingId}/recordings/registrants/status")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
