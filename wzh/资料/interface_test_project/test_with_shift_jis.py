import requests
import json
from datetime import datetime

class ShiftJISTester:
    def __init__(self):
        self.test_url = "http://tes-crasflt.intra.lixil.co.jp/train/sapfront/train/scripts/PrefabTokuchujuchutoroku.asp"
        self.test_data = {
            "InterfaceRequestNumber": "XXXXXXXX",
            "UpdateBy": "1234567890123456",
            "SalesVoucherType": "UN",
            "DeliverySegment": "1",
            "CustomerCd": "201694",
            "PropertyNo": "",
            "Continuation": "",
            "SiteName": "新規",
            "OrderNo": "2233",
            "DeliveryNoteRemarks": "",
            "DeliveryNoteSegment": "",
            "DeliveryExpectDate": "",
            "CustomerOrderNo": "",
            "TaxDivision": "",
            "ShipmentDestinationCd": "127223",
            "ShipmentSegment": "51",
            "DepartmentCd": "JTX7",
            "AllocationDestination": "",
            "List": [
                {
                    "RepresentativeCd": "TAALG3G16FL3G",
                    "Quantity": "10",
                    "UnitPrice": "1000",
                    "BusinessDivision": "100",
                    "ItemSummary": "1222",
                    "Symbol1": "HG",
                    "Dimension1": "1000",
                    "Symbol2": "WG",
                    "Dimension2": "500",
                    "Symbol3": "",
                    "Dimension3": "",
                    "Symbol4": "",
                    "Dimension4": "",
                    "Symbol5": "",
                    "Dimension5": "",
                    "Symbol6": "",
                    "Dimension6": "",
                    "ApprovalNoOutOfLimit": "",
                    "CustomerCd": "テスト新規",
                    "Row": 1
                }
            ]
        }

    def test_with_shift_jis(self):
        """使用Shift_JIS编码测试"""
        print(f"[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 使用Shift_JIS编码测试...")
        try:
            # 将JSON数据转换为字符串
            json_string = json.dumps(self.test_data, ensure_ascii=False)
            print(f"JSON字符串: {json_string}")
            
            # 使用Shift_JIS编码
            encoded_data = json_string.encode('shift_jis')
            print(f"编码后数据长度: {len(encoded_data)} bytes")
            
            headers = {
                'Content-Type': 'application/x-www-form-urlencoded',
                'User-Agent': 'InterfaceTester/1.0'
            }
            
            response = requests.post(
                self.test_url,
                data=encoded_data,
                headers=headers,
                timeout=30
            )
            
            print(f"HTTP状态码: {response.status_code}")
            print(f"响应头: {dict(response.headers)}")
            
            if response.status_code == 200:
                print(f"✅ 成功! 响应内容: {response.text[:1000]}")
                return {
                    'success': True,
                    'response': response.text,
                    'status_code': response.status_code
                }
            else:
                print(f"❌ 失败! 响应内容: {response.text[:1000]}")
                return {
                    'success': False,
                    'error': f'HTTP错误: {response.status_code}',
                    'response': response.text
                }
                
        except Exception as e:
            print(f"❌ 异常: {str(e)}")
            return {
                'success': False,
                'error': str(e)
            }

    def test_with_form_data_shift_jis(self):
        """使用表单数据和Shift_JIS编码"""
        print(f"\n[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 使用表单数据和Shift_JIS编码...")
        try:
            # 将JSON数据作为表单字段值
            json_string = json.dumps(self.test_data, ensure_ascii=False)
            form_data = f"data={json_string}".encode('shift_jis')
            
            headers = {
                'Content-Type': 'application/x-www-form-urlencoded',
                'User-Agent': 'InterfaceTester/1.0'
            }
            
            response = requests.post(
                self.test_url,
                data=form_data,
                headers=headers,
                timeout=30
            )
            
            print(f"HTTP状态码: {response.status_code}")
            
            if response.status_code == 200:
                print(f"✅ 成功! 响应内容: {response.text[:1000]}")
                return {
                    'success': True,
                    'response': response.text,
                    'status_code': response.status_code
                }
            else:
                print(f"❌ 失败! 响应内容: {response.text[:1000]}")
                return {
                    'success': False,
                    'error': f'HTTP错误: {response.status_code}',
                    'response': response.text
                }
                
        except Exception as e:
            print(f"❌ 异常: {str(e)}")
            return {
                'success': False,
                'error': str(e)
            }

    def test_direct_json_shift_jis(self):
        """直接发送JSON数据使用Shift_JIS编码"""
        print(f"\n[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 直接发送JSON数据使用Shift_JIS编码...")
        try:
            json_string = json.dumps(self.test_data, ensure_ascii=False)
            encoded_data = json_string.encode('shift_jis')
            
            headers = {
                'Content-Type': 'application/json; charset=shift_jis',
                'User-Agent': 'InterfaceTester/1.0'
            }
            
            response = requests.post(
                self.test_url,
                data=encoded_data,
                headers=headers,
                timeout=30
            )
            
            print(f"HTTP状态码: {response.status_code}")
            
            if response.status_code == 200:
                print(f"✅ 成功! 响应内容: {response.text[:1000]}")
                return {
                    'success': True,
                    'response': response.text,
                    'status_code': response.status_code
                }
            else:
                print(f"❌ 失败! 响应内容: {response.text[:1000]}")
                return {
                    'success': False,
                    'error': f'HTTP错误: {response.status_code}',
                    'response': response.text
                }
                
        except Exception as e:
            print(f"❌ 异常: {str(e)}")
            return {
                'success': False,
                'error': str(e)
            }

def main():
    tester = ShiftJISTester()
    
    print("=" * 80)
    print("使用Shift_JIS编码的接口测试")
    print("=" * 80)
    print(f"目标URL: {tester.test_url}")
    print(f"测试数据: {json.dumps(tester.test_data, ensure_ascii=False, indent=2)}")
    
    results = {}
    
    results['shift_jis_raw'] = tester.test_with_shift_jis()
    results['shift_jis_form'] = tester.test_with_form_data_shift_jis()
    results['shift_jis_json'] = tester.test_direct_json_shift_jis()
    
    print("\n" + "=" * 80)
    print("测试总结")
    print("=" * 80)
    
    success_count = 0
    for test_name, result in results.items():
        if result.get('success'):
            status = "✅ 成功"
            success_count += 1
        else:
            status = "❌ 失败"
        print(f"{test_name}: {status}")
    
    print(f"\n成功测试数量: {success_count}/{len(results)}")
    
    if success_count > 0:
        print("\n🎉 问题已解决! 关键发现:")
        print("- 接口需要使用 Shift_JIS 编码")
        print("- Content-Type: application/x-www-form-urlencoded")
        print("- 需要将JSON数据转换为字节数组发送")
    else:
        print("\n💡 建议:")
        print("1. 检查Shift_JIS编码是否正确")
        print("2. 确认接口期望的数据格式")
        print("3. 联系开发团队获取准确的接口文档")

if __name__ == "__main__":
    main()
