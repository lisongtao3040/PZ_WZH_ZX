import requests
import json
import time
import threading
from datetime import datetime
import os

class InterfaceTesterFixed:
    def __init__(self):
        self.test_url = "http://tes-crasflt.intra.lixil.co.jp/train/sapfront/train/scripts/PrefabTokuchujuchutoroku.asp"
        self.monitor_url = "http://lxjtddsap177.lixil.lan/train/common/scripts/trainmonitor.asp"
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
        self.expected_result = {
            "intRcd": 0,
            "strMsg": "正常終了",
            "PropertyNo": ""
        }
        self.monitor_changes = []
        self.test_result = None

    def test_interface_form_data(self):
        """使用表单数据格式测试接口"""
        print(f"[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 开始测试接口(表单数据格式)...")
        print(f"目标URL: {self.test_url}")
        
        try:
            # 尝试使用表单数据格式
            headers = {
                'Content-Type': 'application/x-www-form-urlencoded',
                'User-Agent': 'InterfaceTester/1.0'
            }
            
            # 将JSON数据转换为表单格式
            form_data = f"data={json.dumps(self.test_data, ensure_ascii=False)}"
            
            response = requests.post(
                self.test_url, 
                data=form_data,
                headers=headers,
                timeout=30
            )
            
            print(f"HTTP状态码: {response.status_code}")
            print(f"响应头: {dict(response.headers)}")
            
            if response.status_code == 200:
                try:
                    result = response.json()
                    self.test_result = {
                        'success': True,
                        'response': result,
                        'status_code': response.status_code,
                        'response_time': response.elapsed.total_seconds()
                    }
                    print(f"接口响应: {json.dumps(result, ensure_ascii=False, indent=2)}")
                except json.JSONDecodeError:
                    self.test_result = {
                        'success': False,
                        'error': '响应不是有效的JSON格式',
                        'raw_response': response.text[:1000],
                        'status_code': response.status_code
                    }
                    print(f"响应内容(前1000字符): {response.text[:1000]}")
            else:
                self.test_result = {
                    'success': False,
                    'error': f'HTTP错误: {response.status_code}',
                    'status_code': response.status_code,
                    'raw_response': response.text[:1000] if response.text else '无响应内容'
                }
                print(f"错误响应: {response.text[:1000] if response.text else '无响应内容'}")
                
        except requests.exceptions.RequestException as e:
            self.test_result = {
                'success': False,
                'error': f'请求异常: {str(e)}',
                'status_code': None
            }
            print(f"请求异常: {str(e)}")
        
        return self.test_result

    def test_interface_json_with_charset(self):
        """使用JSON格式但指定字符集测试接口"""
        print(f"[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 开始测试接口(JSON格式+字符集)...")
        print(f"目标URL: {self.test_url}")
        
        try:
            headers = {
                'Content-Type': 'application/json; charset=utf-8',
                'User-Agent': 'InterfaceTester/1.0'
            }
            
            response = requests.post(
                self.test_url, 
                json=self.test_data, 
                headers=headers,
                timeout=30
            )
            
            print(f"HTTP状态码: {response.status_code}")
            print(f"响应头: {dict(response.headers)}")
            
            if response.status_code == 200:
                try:
                    result = response.json()
                    self.test_result = {
                        'success': True,
                        'response': result,
                        'status_code': response.status_code,
                        'response_time': response.elapsed.total_seconds()
                    }
                    print(f"接口响应: {json.dumps(result, ensure_ascii=False, indent=2)}")
                except json.JSONDecodeError:
                    self.test_result = {
                        'success': False,
                        'error': '响应不是有效的JSON格式',
                        'raw_response': response.text[:1000],
                        'status_code': response.status_code
                    }
                    print(f"响应内容(前1000字符): {response.text[:1000]}")
            else:
                self.test_result = {
                    'success': False,
                    'error': f'HTTP错误: {response.status_code}',
                    'status_code': response.status_code,
                    'raw_response': response.text[:1000] if response.text else '无响应内容'
                }
                print(f"错误响应: {response.text[:1000] if response.text else '无响应内容'}")
                
        except requests.exceptions.RequestException as e:
            self.test_result = {
                'success': False,
                'error': f'请求异常: {str(e)}',
                'status_code': None
            }
            print(f"请求异常: {str(e)}")
        
        return self.test_result

    def test_interface_text_plain(self):
        """使用纯文本格式测试接口"""
        print(f"[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 开始测试接口(纯文本格式)...")
        print(f"目标URL: {self.test_url}")
        
        try:
            headers = {
                'Content-Type': 'text/plain; charset=utf-8',
                'User-Agent': 'InterfaceTester/1.0'
            }
            
            response = requests.post(
                self.test_url, 
                data=json.dumps(self.test_data, ensure_ascii=False),
                headers=headers,
                timeout=30
            )
            
            print(f"HTTP状态码: {response.status_code}")
            print(f"响应头: {dict(response.headers)}")
            
            if response.status_code == 200:
                try:
                    result = response.json()
                    self.test_result = {
                        'success': True,
                        'response': result,
                        'status_code': response.status_code,
                        'response_time': response.elapsed.total_seconds()
                    }
                    print(f"接口响应: {json.dumps(result, ensure_ascii=False, indent=2)}")
                except json.JSONDecodeError:
                    self.test_result = {
                        'success': False,
                        'error': '响应不是有效的JSON格式',
                        'raw_response': response.text[:1000],
                        'status_code': response.status_code
                    }
                    print(f"响应内容(前1000字符): {response.text[:1000]}")
            else:
                self.test_result = {
                    'success': False,
                    'error': f'HTTP错误: {response.status_code}',
                    'status_code': response.status_code,
                    'raw_response': response.text[:1000] if response.text else '无响应内容'
                }
                print(f"错误响应: {response.text[:1000] if response.text else '无响应内容'}")
                
        except requests.exceptions.RequestException as e:
            self.test_result = {
                'success': False,
                'error': f'请求异常: {str(e)}',
                'status_code': None
            }
            print(f"请求异常: {str(e)}")
        
        return self.test_result

    def monitor_page(self, duration=10):
        """监控页面变化"""
        print(f"\n[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] 开始监控页面，持续{duration}秒...")
        print(f"监控URL: {self.monitor_url}")
        
        start_time = time.time()
        check_count = 0
        
        while time.time() - start_time < duration:
            try:
                response = requests.get(self.monitor_url, timeout=5)
                check_count += 1
                
                monitor_data = {
                    'timestamp': datetime.now().strftime('%Y-%m-%d %H:%M:%S'),
                    'status_code': response.status_code,
                    'content_length': len(response.text),
                    'check_count': check_count
                }
                
                self.monitor_changes.append(monitor_data)
                print(f"监控检查 #{check_count}: 状态码={response.status_code}, 内容长度={len(response.text)}")
                
                time.sleep(2)  # 每2秒检查一次
                
            except requests.exceptions.RequestException as e:
                monitor_data = {
                    'timestamp': datetime.now().strftime('%Y-%m-%d %H:%M:%S'),
                    'status_code': 'ERROR',
                    'error': str(e),
                    'check_count': check_count
                }
                self.monitor_changes.append(monitor_data)
                print(f"监控检查 #{check_count}: 错误 - {str(e)}")
                time.sleep(2)

    def run_comprehensive_test(self):
        """运行全面的测试"""
        print("=" * 80)
        print("全面接口测试与监控开始")
        print("=" * 80)
        
        # 创建并启动监控线程
        monitor_thread = threading.Thread(target=self.monitor_page, args=(10,))
        monitor_thread.start()
        
        # 等待1秒让监控开始
        time.sleep(1)
        
        # 尝试不同的数据格式
        print("\n1. 尝试表单数据格式...")
        result1 = self.test_interface_form_data()
        
        if not result1.get('success'):
            print("\n2. 尝试JSON格式+字符集...")
            result2 = self.test_interface_json_with_charset()
            
            if not result2.get('success'):
                print("\n3. 尝试纯文本格式...")
                result3 = self.test_interface_text_plain()
        
        # 等待监控线程结束
        monitor_thread.join()
        
        return self.test_result

    def generate_report(self):
        """生成测试报告"""
        print("\n" + "=" * 80)
        print("测试报告")
        print("=" * 80)
        
        report = {
            'test_timestamp': datetime.now().strftime('%Y-%m-%d %H:%M:%S'),
            'test_url': self.test_url,
            'monitor_url': self.monitor_url,
            'test_data': self.test_data,
            'expected_result': self.expected_result,
            'actual_result': self.test_result,
            'monitor_results': self.monitor_changes,
            'summary': {}
        }
        
        # 分析测试结果
        if self.test_result and self.test_result.get('success'):
            actual_response = self.test_result.get('response', {})
            expected_keys = ['intRcd', 'strMsg', 'PropertyNo']
            
            matches = True
            for key in expected_keys:
                if key in actual_response and key in self.expected_result:
                    if actual_response[key] == self.expected_result[key]:
                        print(f"✓ {key}: 匹配 (期望: {self.expected_result[key]}, 实际: {actual_response[key]})")
                    else:
                        print(f"✗ {key}: 不匹配 (期望: {self.expected_result[key]}, 实际: {actual_response[key]})")
                        matches = False
                else:
                    print(f"? {key}: 字段缺失")
                    matches = False
            
            report['summary']['test_passed'] = matches
            report['summary']['response_time'] = self.test_result.get('response_time')
        else:
            report['summary']['test_passed'] = False
            report['summary']['error'] = self.test_result.get('error') if self.test_result else '测试未执行'
        
        # 分析监控结果
        if self.monitor_changes:
            successful_checks = [m for m in self.monitor_changes if m.get('status_code') == 200]
            failed_checks = [m for m in self.monitor_changes if m.get('status_code') != 200]
            
            report['summary']['monitor_checks'] = len(self.monitor_changes)
            report['summary']['successful_monitor_checks'] = len(successful_checks)
            report['summary']['failed_monitor_checks'] = len(failed_checks)
            
            print(f"\n监控统计:")
            print(f"  总检查次数: {len(self.monitor_changes)}")
            print(f"  成功次数: {len(successful_checks)}")
            print(f"  失败次数: {len(failed_checks)}")
        
        # 保存报告到文件
        report_filename = f"test_report_fixed_{datetime.now().strftime('%Y%m%d_%H%M%S')}.json"
        report_path = os.path.join(os.path.dirname(__file__), report_filename)
        
        with open(report_path, 'w', encoding='utf-8') as f:
            json.dump(report, f, ensure_ascii=False, indent=2)
        
        print(f"\n详细报告已保存到: {report_path}")
        return report

def main():
    tester = InterfaceTesterFixed()
    
    # 运行全面测试
    test_result = tester.run_comprehensive_test()
    
    # 生成报告
    report = tester.generate_report()
    
    # 输出最终结论
    print("\n" + "=" * 80)
    print("最终结论")
    print("=" * 80)
    
    if report['summary'].get('test_passed'):
        print("✅ 接口测试: 通过")
    else:
        print("❌ 接口测试: 失败")
        print(f"错误信息: {report['summary'].get('error', '未知错误')}")
    
    monitor_success_rate = report['summary'].get('successful_monitor_checks', 0) / max(report['summary'].get('monitor_checks', 1), 1)
    if monitor_success_rate >= 0.8:
        print("✅ 页面监控: 正常")
    else:
        print("⚠️  页面监控: 存在问题")
    
    print(f"响应时间: {report['summary'].get('response_time', 'N/A')}秒")

if __name__ == "__main__":
    main()
