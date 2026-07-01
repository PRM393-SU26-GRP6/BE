# Hướng dẫn tích hợp Bản đồ Vietmap (Flutter)

Backend đã cập nhật API, bảng `Venue` bây giờ đã có thêm 2 trường tọa độ: `Latitude` (Vĩ độ) và `Longitude` (Kinh độ).

> **Lưu ý Quan Trọng:**
> - Các sân bóng đã tạo trước đây sẽ có tọa độ mặc định là `(0, 0)`. Chủ sân cần vào giao diện chỉnh sửa để cập nhật lại tọa độ chính xác.
> - API Tạo Sân (`POST /api/v1/owner/venues`) và Cập nhật Sân (`PUT /api/v1/owner/venues/{id}`) giờ đây yêu cầu gửi kèm 2 trường `latitude` và `longitude` kiểu `double`.

---

## 1. Cập nhật Model trong Flutter

Bạn cần cập nhật `VenueModel` (hoặc `VenueDto`) trong Flutter để hứng dữ liệu trả về từ Backend:

```dart
class VenueModel {
  final String venueId;
  final String venueName;
  final String address;
  final double latitude;   // <-- Thêm trường này
  final double longitude;  // <-- Thêm trường này
  final String description;
  
  // ... constructor ...

  factory VenueModel.fromJson(Map<String, dynamic> json) {
    return VenueModel(
      venueId: json['venueId'] ?? '',
      venueName: json['venueName'] ?? '',
      address: json['address'] ?? '',
      latitude: json['latitude']?.toDouble() ?? 0.0,    // Parse cẩn thận kiểu double
      longitude: json['longitude']?.toDouble() ?? 0.0,
      description: json['description'] ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'venueName': venueName,
      'address': address,
      'latitude': latitude,    // Gửi lên Backend khi tạo/sửa sân
      'longitude': longitude,  // Gửi lên Backend khi tạo/sửa sân
      'description': description,
    };
  }
}
```

---

## 2. Tích hợp Vietmap GL vào Flutter

Để hiển thị bản đồ Vietmap, bạn sử dụng package [vietmap_flutter_gl](https://pub.dev/packages/vietmap_flutter_gl).

### Cài đặt
Thêm vào `pubspec.yaml`:
```yaml
dependencies:
  vietmap_flutter_gl: ^latest_version
```

### Cách lấy API Key
Bạn cần đăng ký tài khoản trên [Vietmap API Portal](https://maps.vietmap.vn/) để lấy `apikey`.

### Code Mẫu hiển thị Bản đồ và Ghim Sân Bóng (Marker)

```dart
import 'dart:math';
import 'package:flutter/material.dart';
import 'package:vietmap_flutter_gl/vietmap_flutter_gl.dart';

class VenueMapView extends StatefulWidget {
  final double venueLat;
  final double venueLng;
  final String venueName;

  const VenueMapView({
    Key? key, 
    required this.venueLat, 
    required this.venueLng,
    required this.venueName,
  }) : super(key: key);

  @override
  State<VenueMapView> createState() => _VenueMapViewState();
}

class _VenueMapViewState extends State<VenueMapView> {
  VietmapController? _mapController;
  final String _vietmapApiKey = "YOUR_VIETMAP_API_KEY_HERE";

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Vị trí Sân bóng')),
      body: Stack(
        children: [
          VietmapGL(
            // Dùng style Vietmap mặc định
            styleString: 'https://maps.vietmap.vn/api/maps/light/styles.json?apikey=$_vietmapApiKey',
            initialCameraPosition: CameraPosition(
              target: LatLng(widget.venueLat, widget.venueLng), // Tọa độ lấy từ API
              zoom: 15.0,
            ),
            onMapCreated: (VietmapController controller) {
              setState(() {
                _mapController = controller;
              });
            },
            onMapRenderedCallback: () {
              // Khi load map xong, ghim 1 cái cờ (Marker) vào vị trí sân
              _addVenueMarker();
            },
          ),
        ],
      ),
    );
  }

  void _addVenueMarker() async {
    if (_mapController == null) return;
    
    await _mapController!.addSymbol(
      SymbolOptions(
        geometry: LatLng(widget.venueLat, widget.venueLng),
        iconImage: "marker-15", // Icon mặc định của Vietmap, hoặc bạn có thể dùng custom icon
        iconSize: 1.5,
        textField: widget.venueName,
        textOffset: Offset(0, 1.5),
        textColor: '#000000',
      ),
    );
  }
}
```

---

## 3. Cho phép Chủ sân chọn vị trí (Pick Location)

Trong màn hình **Tạo/Sửa Sân Bóng**, khi chủ sân chạm vào bản đồ, bạn bắt sự kiện `onMapClick` để lấy tọa độ mới và gán vào request body trước khi gọi API `POST /api/v1/owner/venues`:

```dart
VietmapGL(
  styleString: 'https://maps.vietmap.vn/api/maps/light/styles.json?apikey=...',
  initialCameraPosition: CameraPosition(
    target: LatLng(10.762622, 106.660172), // Tọa độ mặc định (ví dụ TPHCM)
    zoom: 12.0,
  ),
  onMapClick: (Point<double> point, LatLng coordinates) {
    print("Chủ sân đã chọn: ${coordinates.latitude}, ${coordinates.longitude}");
    // Lưu tọa độ này vào biến state để lúc bấm nút "Lưu sân bóng", 
    // bạn gửi nó qua JSON (latitude, longitude) lên API.
  },
)
```

**Mẹo:** 
Bạn có thể tích hợp thêm API **Geocoding của Vietmap** (`https://maps.vietmap.vn/api/search/v3?apikey=...&text={địa_chỉ}`) để khi chủ sân gõ địa chỉ vào Textfield, ứng dụng tự động đổi thành Lat/Lng và bay bản đồ tới vị trí đó!
