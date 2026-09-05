class BaseResponse {
  final String? response;
  final bool success;
  final String? message;

  BaseResponse(this.response, this.success, {this.message});
}
