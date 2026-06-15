import requests
import json
import time
from datetime import datetime
import os

class DetailedInterfaceTester:
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

    def test_get_request(self):
        """测试GET请求"""
        print(f"[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 测试GET请求...")
        try:
            response = requests.get(self.test_url, timeout=10)
            print(f"GET请求状态码: {response.status_code}")
            print(f"响应内容长度: {len(response.text)}")
            print(f"响应内容前500字符: {response.text[:500]}")
            return response
        except Exception as e:
            print(f"GET请求异常: {str(e)}")
            return None

    def test_post_with_query_params(self):
        """测试POST请求但使用查询参数"""
        print(f"\n[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 测试POST请求(查询参数)...")
        try:
            # 将JSON数据作为查询参数
            query_params = {"data": json.dumps(self.test_data, ensure_ascii=False)}
            response = requests.post(self.test_url, params=query_params, timeout=10)
            print(f"POST请求(查询参数)状态码: {response.status_code}")
            print(f"响应内容长度: {len(response.text)}")
            print(f"响应内容前500字符: {response.text[:500]}")
            return response
        except Exception as e:
            print(f"POST请求(查询参数)异常: {str(e)}")
            return None

    def test_post_with_form_data(self):
        """测试POST请求使用表单数据"""
        print(f"\n[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 测试POST请求(表单数据)...")
        try:
            # 尝试不同的表单字段名
            form_fields = ["data", "json", "request", "input"]
            
            for field_name in form_fields:
                print(f"尝试表单字段名: {field_name}")
                form_data = {field_name: json.dumps(self.test_data, ensure_ascii=False)}
                response = requests.post(
                    self.test_url, 
                    data=form_data,
                    headers={'Content-Type': 'application/x-www-form-urlencoded'},
                    timeout=10
                )
                print(f"状态码: {response.status_code}")
                if response.status_code == 200:
                    print(f"成功! 使用字段名: {field_name}")
                    print(f"响应内容: {response.text[:1000]}")
                    return response
                else:
                    print(f"失败，继续尝试其他字段名...")
            
            return response
        except Exception as e:
            print(f"POST请求(表单数据)异常: {str(e)}")
            return None

    def test_simplified_data(self):
        """测试简化数据"""
        print(f"\n[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 测试简化数据...")
        try:
            simplified_data = {
                "InterfaceRequestNumber": "TEST001",
                "UpdateBy": "1234567890123456",
                "SalesVoucherType": "UN",
                "CustomerCd": "201694",
                "OrderNo": "2233"
            }
            
            headers = {
                'Content-Type': 'application/json',
                'User-Agent': 'InterfaceTester/1.0'
            }
            
            response = requests.post(
                self.test_url, 
                json=simplified_data,
                headers=headers,
                timeout=10
            )
            print(f"简化数据测试状态码: {response.status_code}")
            print(f"响应内容: {response.text[:1000]}")
            return response
        except Exception as e:
            print(f"简化数据测试异常: {str(e)}")
            return None

    def test_without_list(self):
        """测试不带List字段的数据"""
        print(f"\n[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 测试不带List字段的数据...")
        try:
            data_without_list = {
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
                "AllocationDestination": ""
            }
            
            headers = {
                'Content-Type': 'application/json',
                'User-Agent': 'InterfaceTester/1.0'
            }
            
            response = requests.post(
                self.test_url, 
                json=data_without_list,
                headers=headers,
                timeout=10
            )
            print(f"不带List字段测试状态码: {response.status_code}")
            print(f"响应内容: {response.text[:1000]}")
            return response
        except Exception as e:
            print(f"不带List字段测试异常: {str(e)}")
            return None

    def run_all_tests(self):
        """运行所有测试"""
        print("=" * 80)
        print("详细接口测试开始")
        print("=" * 80)
        print(f"目标URL: {self.test_url}")
        print(f"测试数据: {json.dumps(self.test_data, ensure_ascii=False, indent=2)}")
        
        results = {}
        
        # 测试GET请求
        results['get'] = self.test_get_request()
        
        # 测试POST请求(查询参数)
        results['post_query'] = self.test_post_with_query_params()
        
        # 测试POST请求(表单数据)
        results['post_form'] = self.test_post_with_form_data()
        
        # 测试简化数据
        results['simplified'] = self.test_simplified_data()
        
        # 测试不带List字段的数据
        results['without_list'] = self.test_without_list()
        
        return results

def main():
    tester = DetailedInterfaceTester()
    results = tester.run_all_tests()
    
    print("\n" + "=" * 80)
    print("测试总结")
    print("=" * 80)
    
    for test_name, result in results.items():
        if result:
            status = "✅ 成功" if result.status_code == 200 else "❌ 失败"
            print(f"{test_name}: {status} (状态码: {result.status_code})")
        else:
            print(f"{test_name}: ❌ 无响应")

if __name__ == "__main__":
    main()
